import http from 'k6/http';
import { check, sleep } from 'k6';

// Set your API URL here
const apiUrl = 'http://localhost:5001/api/todoItems';
const token = 'eyJhbGciOiJSUzI1NiIsImtpZCI6IjNmOWEwNTBkYzRhZTgyOGMyODcxYzMyNTYzYzk5ZDUwMjc3ODRiZTUiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoiYWthbGFua2EgZGlzc2FuYXlha2UiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUNnOG9jSWQ1SGtKRTZGNVJLTUVWZGEyODI3OFdmZTkyU3FRRURieEtfWGcySTJ6aUNlUWhiY2k9czk2LWMiLCJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vYmFzZXRlbXBsYXRlLWZiODkyIiwiYXVkIjoiYmFzZXRlbXBsYXRlLWZiODkyIiwiYXV0aF90aW1lIjoxNzQ0NDU4NzYxLCJ1c2VyX2lkIjoiaTUzTUVsMHk3ak93SWgwQnZtSHFkMFBNRG5mMiIsInN1YiI6Imk1M01FbDB5N2pPd0loMEJ2bUhxZDBQTURuZjIiLCJpYXQiOjE3NDYwOTc1NzksImV4cCI6MTc0NjEwMTE3OSwiZW1haWwiOiJkaXNhMmFrYUBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJnb29nbGUuY29tIjpbIjEwMjA2MTM1NjM2MDk0MDMwNDkyNiJdLCJlbWFpbCI6WyJkaXNhMmFrYUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJnb29nbGUuY29tIn19.QU89uLZbsWpXeL4vkmPhblo8N6zKPpF4lnLTmnMrvFUgqahxTWUMhxQ-_hHFMcPdTSo04Jpt65GNVUC0-nXUZKrF8Nhg3WnNKJoNmQQJVI_4JxbY1WnfPs4qA0Uk0v-3J0sNspttQf01y-2m2nx7hUXCDiJ3kJQB1ior16ZLivZX8_AEkq55J5AwisqXHXR6O7HqR5hJlOa66BGis0wZ1GaC_tEjH6DuDSd0trTi_NFWavrqjzWhligNJjIKUjDfSTRu3OsKiS7mGwXl4IRTPI0PkgPVIw__Umpx4G8EoblUDjcnG-eFt-sjr_UQZ7GQ8X2hn7YNNBZp_qS1obGxYw'; // Replace with your actual Bearer token

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