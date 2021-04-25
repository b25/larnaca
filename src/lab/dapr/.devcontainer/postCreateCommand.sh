docker-compose -f .devcontainer/grafana/docker-compose.yml up -d
wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | /bin/bash
dapr init
