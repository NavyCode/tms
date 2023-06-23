FROM nginx:latest
WORKDIR /app    
RUN rm -rf /usr/share/nginx/html/*
COPY ./nginx/nginx.conf /etc/nginx/nginx.conf
COPY ./dist/bg /usr/share/nginx/html
EXPOSE 80

