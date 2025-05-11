// JavaScript source code
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '3s', target: 250 },
    { duration: '3s', target: 500 },
    { duration: '3s', target: 1000 },
    { duration: '3s', target: 2500 },
    { duration: '3s', target: 4000 },
    { duration: '3s', target: 6000 },
  ],
};

// Set your API URL here
const apiUrl = 'http://localhost:5005/api/todoItems';
const token = 'eyJhbGciOiJSUzI1NiIsImtpZCI6IjU5MWYxNWRlZTg0OTUzNjZjOTgyZTA1MTMzYmNhOGYyNDg5ZWFjNzIiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoiYWthbGFua2EgZGlzc2FuYXlha2UiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUNnOG9jSWQ1SGtKRTZGNVJLTUVWZGEyODI3OFdmZTkyU3FRRURieEtfWGcySTJ6aUNlUWhiY2k9czk2LWMiLCJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vYmFzZXRlbXBsYXRlLWZiODkyIiwiYXVkIjoiYmFzZXRlbXBsYXRlLWZiODkyIiwiYXV0aF90aW1lIjoxNzQ0NDU4NzYxLCJ1c2VyX2lkIjoiaTUzTUVsMHk3ak93SWgwQnZtSHFkMFBNRG5mMiIsInN1YiI6Imk1M01FbDB5N2pPd0loMEJ2bUhxZDBQTURuZjIiLCJpYXQiOjE3NDY5NDI4MTMsImV4cCI6MTc0Njk0NjQxMywiZW1haWwiOiJkaXNhMmFrYUBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJnb29nbGUuY29tIjpbIjEwMjA2MTM1NjM2MDk0MDMwNDkyNiJdLCJlbWFpbCI6WyJkaXNhMmFrYUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJnb29nbGUuY29tIn19.N44FsAaXmd9YdA0hbMBVwhOW2phpW27Tqc97aU7RYxDxTRSDRzioKbhoqb0YMBN3f8_OK6Wb0YtCx9wsufq8O_lnDTFLkqM_WZvvt7cKeZbxya534UGcr1NK8-fwrwe6kyhsk7DoLWAoWFF_tW9Zbk_rTyr0N0hMLS9rRsKb-AzYPO16jb07MOqtP2e10rnezofZL1yXQYYhbOcGmPD7vZEiioC9KwX_RGqafa6kcqbJfq_uAtFxYKf8B3xfJ6VxKDCy6lc5zlKeIYxK76t3PzPaMlOmGQgqaM7ymHF0ixNcvxda0txaMktM7YzKsPt7Dm1hn4oxx1H_yuiOIqSGXQ'; // Replace with your actual Bearer token

export default function () {
  // Payload for creating a new Todo item
  const payload = JSON.stringify({
    title: 'yes',
    note: 'dd',
    reminder: '2025-05-07T00:13:00+08:00',
    priority: 2,
    listId: 1
  });

  // Headers with Bearer token for authentication and necessary headers
  const headers = {
    'Authorization': `Bearer ${token}`,  // Authorization header for Bearer token
    'Content-Type': 'application/json',  // Content-Type header for POST request
    'Accept': 'application/json',        // Accept header for JSON response
  };

  // Perform a POST request to create a new Todo item
  let response = http.post(apiUrl, payload, { headers: headers });
  // Check if the response status is 200 and contains an 'id'
  check(response, {
    'status is 200': (r) => r.status === 200
  });

  // Simulate user thinking time between requests
  sleep(1);
}