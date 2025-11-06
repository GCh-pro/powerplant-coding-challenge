
## Architectural stuffs

Api is design within' a mvc ErrorOr pattern. If the posted object is not a full parsable powerplant object it will return an error.  Mapper for DTO object <Tin, Tout>


# Production Planning Algorithm

### Algorithm Steps

1. **Calculate Costs and Available Power**  
   For each power plant, calculate:
   - The **production cost** per MWh.
     - Gas-fired plants: Cost = FuelGas / Efficiency + 0.244 * CO2
     - Kerosene plants: Cost = FuelKerosine / Efficiency + 0.267 * CO2
   - The **available power** (Pmax adjusted for wind):
     - Wind turbines: AvailablePower = Pmax * (WindPercentage / 100)
     - Other plants: AvailablePower = Pmax

2. **Sort by Increasing Cost (Merit Order)**  
   Plants are sorted from cheapest to most expensive to minimize total cost.

3. **Allocate Load**  
   Iterate over the sorted plants:  
   - **Wind turbines** produce up to the remaining load or available power:
     - Production = min(AvailablePower, RemainingLoad)
   - **Gas or Kerosene plants** produce only if remaining load allows at least Pmin:
     - If RemainingLoad >= Pmin: Production = min(AvailablePower, RemainingLoad)
     - Otherwise: Production = 0

4. **Update Remaining Load**  
   After allocating a plant:
   - RemainingLoad = RemainingLoad - Production

5. **Stop Condition**  
   - If RemainingLoad <= 0, stop allocating production.  
   - Unused plants are added with Production = 0.

## How to build : 
=>
docker build -t powerplant-api .    
docker run -p 8888:8080 powerplant-api
after that you can call 
http://localhost:8888/productionplan in POST with the proper json file. 