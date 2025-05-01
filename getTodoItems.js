import http from 'k6/http';
import { check, sleep } from 'k6';

// Set your API URL here
const apiUrl = 'http://localhost:5001/api/TodoItems';
const token = 'eyJhbGciOiJSUzI1NiIsImtpZCI6IjNmOWEwNTBkYzRhZTgyOGMyODcxYzMyNTYzYzk5ZDUwMjc3ODRiZTUiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoiYWthbGFua2EgZGlzc2FuYXlha2UiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUNnOG9jSWQ1SGtKRTZGNVJLTUVWZGEyODI3OFdmZTkyU3FRRURieEtfWGcySTJ6aUNlUWhiY2k9czk2LWMiLCJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vYmFzZXRlbXBsYXRlLWZiODkyIiwiYXVkIjoiYmFzZXRlbXBsYXRlLWZiODkyIiwiYXV0aF90aW1lIjoxNzQ0NDU4NzYxLCJ1c2VyX2lkIjoiaTUzTUVsMHk3ak93SWgwQnZtSHFkMFBNRG5mMiIsInN1YiI6Imk1M01FbDB5N2pPd0loMEJ2bUhxZDBQTURuZjIiLCJpYXQiOjE3NDYwNzI5OTEsImV4cCI6MTc0NjA3NjU5MSwiZW1haWwiOiJkaXNhMmFrYUBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJnb29nbGUuY29tIjpbIjEwMjA2MTM1NjM2MDk0MDMwNDkyNiJdLCJlbWFpbCI6WyJkaXNhMmFrYUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJnb29nbGUuY29tIn19.EKi4MBjSXZfcIBpNVOT5wkJD1vFEl0bsSbe_HQTL3YeyUtl8gcQgCcm9S0JXk0tHT2PD1PnDV7_PYS7g_nODT4Aruv4gOtX9r37BU8sQXt-8wpQTS-_tyUxB6cXSN_O3jnxn5MLGI61tG0VF2gSueMUOx1_CkYBzCDP1xF2y92COrS9K2hlXoV7Tceda_I68VzwhFVKg6uJyKiws90GoAZUBiLJLp5zfBnaTEcYOO4Cc3J9SF9IDPr2JmJSOVShW6cU4ecLs_N9nBKQJ_SbaO1EGbI7JQ8Ox7pOKouynzspFi3s-vE9F97GdqzHXxO9XAW5Weuen9pJ1y7E63LmoLQ'; // Replace this with your actual Bearer token

export default function () {
  // Fetch Todo Items with ListId, PageNumber, and PageSize as query params
  const params = {
    ListId: 1,  // Example List ID
    PageNumber: 1,
    PageSize: 10,
    headers: {
      'Authorization': `Bearer ${token}`,  // Adding Bearer token to the request headers
    },
  };

  let response = http.get(`${apiUrl}?ListId=${params.ListId}&PageNumber=${params.PageNumber}&PageSize=${params.PageSize}`, params);

  // Check if the status is 200 and response is valid
  check(response, {
    'status is 200': (r) => r.status === 200,
    'body contains items': (r) => r.body.length > 0,
  });

  sleep(1);  // Simulate user "thinking" time
}
