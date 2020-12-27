FROM envoyproxy/envoy:v1.16-latest

RUN chmod 500 /docker-entrypoint.sh

RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install gettext -y

COPY envoy.yaml.tmpl /tmpl/envoy.yaml.tmpl
COPY docker-entrypoint.sh /

ENV ADMIN_PORT=90 NODE_ID=envoy1 SERVICE_CONTROLLER_ADDRESS=localhost SERVICE_CONTROLLER_PORT=5000

ENTRYPOINT ["/docker-entrypoint.sh"]