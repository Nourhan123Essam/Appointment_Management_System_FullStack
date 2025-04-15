# Use Node 18 base image
FROM node:18 AS build

# Set working directory
WORKDIR /app

# Install Angular CLI globally
RUN npm install -g @angular/cli

# Copy package.json and package-lock.json first
COPY package*.json ./

# Clean install dependencies
RUN npm ci

# Copy rest of your Angular app
COPY . .

# Start the Angular dev server
CMD ["npm", "start"]
