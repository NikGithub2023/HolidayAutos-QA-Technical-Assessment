# Manual Test Cases

## Test case 1 - Verify a user can successfully search for available rental cars

**Objective**
Verify that a user can search for available rental cars using valid search criteria.

**Preconditions**
- User is on the Holiday Autos home page.

**Test Data**
- Location: Dublin - Airport
- Pickup Date: Current Date
- Pickup Time: At least 30 minutes in the future
- Drop-off Date: 5 days after pickup
- Drop-off Time: Any valid time

**Steps**
1. Enter **Dublin** and select **Dublin - Airport** from the suggested locations.
2. Select the current date as the pickup date.
3. Select a pickup time at least 30 minutes in the future.
4. Select a drop-off date 5 days after the pickup date.
5. Select a valid drop-off time.
6. Click **Search**.

**Expected Result**
• Search results page is displayed.
• At least one rental vehicle is returned.

**Actual Result**
- PASS - Search results page loaded and multiple rental vehicles were displayed.

---

## Test case 2 - Verify the cheapest available rental car can be identified

**Objective**
Verify that the cheapest available rental car can be identified from the search results.

**Preconditions**
- Search results are displayed from MT-001.

**Steps**
1. Locate the **Sort By** option.
2. Select **Price (Low to High)**.
3. Confirm the cars are displayed in ascending price order.
4. Record the price of the first displayed vehicle.

**Expected Result**
- Cars are sorted from the lowest price to the highest.
- The first displayed vehicle represents one of the cheapest available rental options.

**Actual Result**
- PASS - Results were displayed in ascending price order and the cheapest vehicle price was recorded.

---

## Test case 3 - Verify the selected rental car details match the search results

**Objective**
Verify that the selected rental car displays the same details as the search results.

**Preconditions**
- The cheapest rental car has been identified from MT-002.

**Steps**
1. Select the first (cheapest) rental car.
2. Verify the pickup date matches the search criteria.
3. Verify the drop-off date matches the search criteria.
4. Verify the displayed rental price matches the price recorded in MT-002.

**Expected Result**
- Pickup date matches.
- Drop-off date matches.
- Rental price matches the search results.

**Actual Result**
- FAIL - Pickup and drop-off dates matched, but the rental price showed a small decimal variation compared to the search results.
