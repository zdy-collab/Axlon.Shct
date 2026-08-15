FROM nginx:1.29.8-alpine

ENV NGINX_ENVSUBST_FILTER="^(AUTH_HTTP|FILES_HTTP|BASIC_HTTP|MERCHANT_HTTP|ORDER_HTTP)$"

COPY default.conf.template /etc/nginx/templates/default.conf.template
