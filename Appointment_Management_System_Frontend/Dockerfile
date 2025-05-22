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

