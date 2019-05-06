# NetworkUtilities
dotnet core project with a main API which calls worker API's to gather details on IPs and Domains

#### _Example API calls_
https://localhost:44385/api/ip/8.8.8.8

https://localhost:44385/api/ip/8.8.8.8?serviceFilter=geoIp,rdap

https://localhost:44385/api/ip/8.8.8.8?serviceFilter=geoIp

https://localhost:44385/api/domain/google.com

https://localhost:44385/api/domain/google.com?serviceFilter=rdap
