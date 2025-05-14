## Shopping Mart Web App - Marketing MVP

### Overview:

This application is designed as a marketing platform for local stores in a specific area of Sri Lanka. The goal is to highlight store promotions, provide location-based filtering, and give users easy access to local business information without the complexity of inventory management.

### Features:

#### 1. Home Page:

* Display popular stores and featured promotions.
* Categories like Groceries, Electronics, Clothing, and more.
* Highlight time-sensitive offers with countdown timers.

#### 2. Store Directory:

* List of all registered stores.
* Display store details: Address, Contact info, Store timings, and Map location.
* Filters to refine search by Category, Distance, and Current Offers.

#### 3. Store Details Page:

* Show detailed information about the store.
* Display current promotions and special deals.
* High-resolution images of the store and products.
* Integrated map view for easy navigation.
* Share button for easy social media sharing.

#### 4. Promotions Page:

* Aggregated view of all current promotions.
* Search by category or store.
* "Expires Soon" filter for urgent offers.

#### 5. Location-based Filtering:

* Geolocation to filter stores within a specific radius.
* Map view with pins representing store locations.

#### 6. Search Functionality:

* Search for stores, products, or promotions.
* Autocomplete suggestions for quick searching.

#### 7. Basic Admin Portal:

* Allows store owners or administrators to add/edit store information.
* Manage promotions and featured listings.
* View analytics on store page visits and promotions.

---

---

This project is licensed under the MIT License.


C:\Users\Acer\source\test\BaseTemplate>k6 run api-post-k6.js

         /\      Grafana   /‾‾/
    /\  /  \     |\  __   /  /
   /  \/    \    | |/ /  /   ‾‾\
  /          \   |   (  |  (‾)  |
 / __________ \  |_|\_\  \_____/

     execution: local
        script: api-post-k6.js
        output: -

     scenarios: (100.00%) 1 scenario, 6000 max VUs, 48s max duration (incl. graceful stop):
              * default: Up to 6000 looping VUs for 18s over 6 stages (gracefulRampDown: 30s, gracefulStop: 30s)



  █ TOTAL RESULTS

    checks_total.......................: 14418   668.055801/s
    checks_succeeded...................: 100.00% 14418 out of 14418
    checks_failed......................: 0.00%   0 out of 14418

    ✓ status is 200

    HTTP
    http_req_duration.......................................................: avg=2.36s min=521.29µs med=1.26s max=6.57s p(90)=6.2s p(95)=6.39s
      { expected_response:true }............................................: avg=2.36s min=521.29µs med=1.26s max=6.57s p(90)=6.2s p(95)=6.39s
    http_req_failed.........................................................: 0.00%  0 out of 14418
    http_reqs...............................................................: 14418  668.055801/s

    EXECUTION
    iteration_duration......................................................: avg=3.36s min=1s       med=2.27s max=7.57s p(90)=7.2s p(95)=7.39s
    iterations..............................................................: 14418  668.055801/s
    vus.....................................................................: 1941   min=64         max=5899
    vus_max.................................................................: 6000   min=6000       max=6000

    NETWORK
    data_received...........................................................: 3.1 MB 143 kB/s
    data_sent...............................................................: 21 MB  975 kB/s




running (21.6s), 0000/6000 VUs, 14418 complete and 0 interrupted iterations
default ✓ [======================================] 0000/6000 VUs  18s