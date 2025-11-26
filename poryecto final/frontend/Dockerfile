# -----------------------------------------------------
# ETAPA 1: Build de Angular
# -----------------------------------------------------
FROM node:20 AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# -----------------------------------------------------
# ETAPA 2: Servir Angular con NGINX
# -----------------------------------------------------
FROM nginx:alpine

# Copiar configuración de nginx
COPY ./nginx.conf /etc/nginx/conf.d/default.conf

# Copiar el build de Angular
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
