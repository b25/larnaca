#!/bin/sh
set -e

echo "Generating envoy.yaml config file..."
cat /tmpl/envoy.yaml.tmpl | envsubst \$ADMIN_PORT,\$NODE_ID,\$SERVICE_CONTROLLER_ADDRESS,\$SERVICE_CONTROLLER_PORT > /etc/envoy/envoy.yaml

echo "Starting Envoy..."
/usr/local/bin/envoy -c /etc/envoy/envoy.yaml