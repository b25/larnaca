dotnet tool install -g dotnet-grpc-cli

dotnet grpc-cli ls https://localhost:5001
dotnet grpc-cli ls https://localhost:5001 mssql.adapter.demo

//show contract
dotnet grpc-cli dump https://localhost:5001 mssql.adapter.demo
