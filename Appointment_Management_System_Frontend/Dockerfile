# # Use Node 18 base image
# FROM node:18 AS build

# # Set working directory
# WORKDIR /app

# # Install Angular CLI globally
# RUN npm install -g @angular/cli

# # Copy package.json and package-lock.json first
# COPY package*.json ./

# # Clean install dependencies
# RUN npm ci

# # Copy rest of your Angular app
# COPY . .

# # Start the Angular dev server
# CMD ["npm", "start"]

#////////////////////////////////////////////

# Stage 1: Build Angular app
# FROM node:18 AS build

# WORKDIR /app

# # Install Angular CLI globally
# RUN npm install -g @angular/cli

# # Copy dependency files
# COPY package*.json ./

# # Install dependencies
# RUN npm ci

# # Copy full source
# COPY . .

# # Build with Docker environment
# RUN npm run build -- --configuration docker

# # Stage 2: Serve with Nginx
# FROM nginx:alpine

# # Remove default site
# RUN rm -rf /usr/share/nginx/html/*

# # Copy built Angular app
# COPY --from=build /app/dist/my-angular17-app /usr/share/nginx/html

# EXPOSE 80

# CMD ["nginx", "-g", "daemon off;"]


#///////////////////////////////////
# Use Node 18
FROM node:18

WORKDIR /app

# Install Angular CLI
RUN npm install -g @angular/cli

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy app source
COPY . .

# Expose default Angular dev port
EXPOSE 4200

# Start Angular dev server
CMD ["npm", "run", "start:docker"]

