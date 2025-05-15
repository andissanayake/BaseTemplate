# Stage 1: Build the React application
FROM node:18-alpine AS build
WORKDIR /app

# Copy package.json and package-lock.json (or yarn.lock)
COPY package.json ./
COPY package-lock.json ./
# If you are using yarn, uncomment the next two lines and comment out the npm install line
# COPY yarn.lock ./
# RUN yarn install --frozen-lockfile

# Install dependencies
RUN npm install

# Copy the rest of the application source code
COPY . .

# Build the application
RUN npm run build

# Stage 2: Serve the application with Nginx
FROM nginx:stable-alpine
# WORKDIR /usr/share/nginx/html # No longer need to set WORKDIR here as Nginx default is fine for static files

# Remove default Nginx configuration
RUN rm /etc/nginx/conf.d/default.conf

# Copy our custom Nginx configuration
COPY nginx.default.conf /etc/nginx/conf.d/default.conf

# Copy static assets from the build stage to the default Nginx html directory
COPY --from=build /app/dist /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start Nginx and serve files
CMD ["nginx", "-g", "daemon off;"] 