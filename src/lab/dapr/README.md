# Dapr Remote containers environment

The .devcontainer directory contains the configuration for a Dapr environment for [VS Code Remote Containers](https://code.visualstudio.com/docs/remote/containers)

It it based on docker-in-docker configuration; i.e. you open the development container and there is a nested docker where dapr containers are initialized and hosted. These containers are not directly accessible to the host machine, you need to use [port forwarding](https://code.visualstudio.com/docs/remote/containers#_forwarding-or-publishing-a-port) to access them from the host.

After starting allow the 15-30 minutes for environment to spin-up and initialize. Check the terminal & output messages to know when the environment is ready.

## Loki & Graphana 

Loki and Graphana containers are also started with the environment.
check the .devcontainer/grafana folder for 