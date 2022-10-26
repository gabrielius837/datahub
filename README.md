## Running

From root dir

```shell
docker-compose build && docker-compose up
```

This would build and launch postgres and asp.net core web api containers. Database would be populated with [data](https://data.gov.lt/dataset/siame-duomenu-rinkinyje-pateikiami-atsitiktinai-parinktu-1000-buitiniu-vartotoju-automatizuotos-apskaitos-elektriniu-valandiniai-duomenys) of two latest months. Then the final result could be retrieved from [here](http://localhost:5000/regionenergy).