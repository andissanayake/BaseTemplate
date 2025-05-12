// JavaScript source code
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '5s', target: 50 },
    { duration: '5s', target: 100 },
    { duration: '5s', target: 500 },
    { duration: '5s', target: 1000 },
    { duration: '5s', target: 1500 },
    { duration: '5s', target: 2000 },
    { duration: '5s', target: 2500 },
    { duration: '5s', target: 3000 },
    { duration: '5s', target: 3500 },
  ],
};

// Set your API URL here
const apiUrl = 'http://localhost:5005/api/todoItems';
const token = 'eyJhbGciOiJSUzI1NiIsImtpZCI6IjU5MWYxNWRlZTg0OTUzNjZjOTgyZTA1MTMzYmNhOGYyNDg5ZWFjNzIiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoiYWthbGFua2EgZGlzc2FuYXlha2UiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUNnOG9jSWQ1SGtKRTZGNVJLTUVWZGEyODI3OFdmZTkyU3FRRURieEtfWGcySTJ6aUNlUWhiY2k9czk2LWMiLCJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vYmFzZXRlbXBsYXRlLWZiODkyIiwiYXVkIjoiYmFzZXRlbXBsYXRlLWZiODkyIiwiYXV0aF90aW1lIjoxNzQ0NDU4NzYxLCJ1c2VyX2lkIjoiaTUzTUVsMHk3ak93SWgwQnZtSHFkMFBNRG5mMiIsInN1YiI6Imk1M01FbDB5N2pPd0loMEJ2bUhxZDBQTURuZjIiLCJpYXQiOjE3NDcwNTAwNjAsImV4cCI6MTc0NzA1MzY2MCwiZW1haWwiOiJkaXNhMmFrYUBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJnb29nbGUuY29tIjpbIjEwMjA2MTM1NjM2MDk0MDMwNDkyNiJdLCJlbWFpbCI6WyJkaXNhMmFrYUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJnb29nbGUuY29tIn19.Hwfs4NspAgNhLAqwL0jaRL_2hu0W-mcZFX-I32aouUAbdWVy8362_kVWUSEjaSk7rS0WIGpZXeE5kXKFlTjB_UqU1H_2Dhu-1_LGavLR59Lz_ghusapyeiR_Q1T7sGfq1g-zxaUFSZJDPWEYKvW98zMKSXc88BKfiSfO5gPf2BhzVvcoxE5PW-jaHrGTg4HpAOVJvLF_LJoPpCd3V6g3u3ZU0IwqzV1mZIzeNCizz3G4weS-Duz4oxlHg1TNqfTcc9cn0t8Tu9ywr2XXZMlI63U4a7nQZD23EdTgXwBqr1R6_GInLj_nYQh7hKIt_E-o0ow0CgDY5UZj2dUxu7vEaw';

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
  //console.log(response);
  // Check if the response status is 200 and contains an 'id'
  check(response, {
    'status is 200': (r) => r.status === 200
  });

  // Simulate user thinking time between requests
  sleep(1);
}