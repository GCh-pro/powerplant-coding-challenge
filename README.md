
## Architectural stuffs

Api is design within' a mvc ErrorOr pattern. If the posted object is not a full parsable powerplant object it will return an error.  Mapper for DTO object <Tin, Tout>


## Algorithm Steps

1. **Calculate Costs and Available Power**  
   For each power plant, calculate:
   - The **production cost** per MWh.
   - The **available power** (Pmax adjusted for wind).

2. **Sort by Increasing Cost (Merit Order)**  
   Plants are sorted from cheapest to most expensive to minimize total cost.

3. **Allocate Load**  
   Iterate over the sorted plants:  
   - **Wind turbines** produce up to the remaining load or available power:

   \[
   P_{\text{wind}} = \min(P_{\text{max}} \cdot \frac{\text{Wind}}{100}, \text{Remaining Load})
   \]

   - **Gas or Kerosene plants** produce only if remaining load allows at least Pmin:

   \[
   P_{\text{plant}} =
   \begin{cases}
   \min(P_{\text{max}}, \text{Remaining Load}) & \text{if Remaining Load} \ge P_{\min} \\
   0 & \text{otherwise}
   \end{cases}
   \]

4. **Update Remaining Load**  
   After allocating a plant:

   \[
   \text{Remaining Load} = \text{Remaining Load} - P_{\text{plant}}
   \]

5. **Stop Condition**  
   - If the remaining load ≤ 0, stop allocating production.  
   - Unused plants are added with \(P = 0\).

---

### Cost Formulas (Reference)

- **Gas-fired plants**:

\[
C_{\text{gas}} = \frac{\text{Fuel}_{\text{Gas}}}{\text{Efficiency}} + 0.244 \cdot \text{CO}_2
\]

- **Kerosene plants**:

\[
C_{\text{kerosene}} = \frac{\text{Fuel}_{\text{Kerosine}}}{\text{Efficiency}} + 0.267 \cdot \text{CO}_2
\]

- **Wind turbines**:

\[
P_{\text{wind}} = P_{\text{max}} \cdot \frac{\text{Wind\%}}{100}
\]


## How to build : 
=>
docker build -t powerplant-api .    
docker run -p 8888:8080 powerplant-api
after that you can call 
http://localhost:8888/productionplan in POST with the proper json file. 