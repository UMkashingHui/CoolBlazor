<p align="center">
   <img src="/CoolBlazor/CoolBlazor/wwwroot/images/Logos/svglogo.svg" alt="CoolBlazor" title="CoolBlazor">
</p>

# Description
This repository has two projects which are CoolBlazor and CoolWebApi. CoolBlazor is a front-end  application built with Blazor Server technology and MudBlazor UI, while CoolWebApi using .Net Core Web Api to support the back-end work of CoolBlazor. To develop web application faster, CoolWebApi uses MongoDB database instead of sql server. MongoDB is a NoSQL database which is powerful at handling lots of data and easier to maintain than traditional database. 

Including:
1. Register and Login with Microsoft Identity service and MongoDB.
2. Authentication and authorization with Microsoft Identity service and MongoDB. 
3. Modify personal information and crop photo as avatar powered by Cropper.Blazor.
4. Microsoft AI support. (TODO)
5. A markdown blog and article display platform. (TODO)

# Live Demo
[coolblazor.space](https://www.coolblazor.space)

# Development Setup
- Microsoft Visual Studio Code
- .Net 7
- MongoDB<br>
     Get your Connection String! Ref:https://www.mongodb.com/docs/manual/reference/connection-string/
- MudBlazor

# Docker Setup for CoolBlazor and CoolWebApi
## Build Docker Images
```
# Build CoolBlazor Image
sudo docker build --no-cache -t blazor-server-with-docker .

# Build CoolWebApi Image
sudo docker build -t web-api-with-docker .
```
## Run the Images
```
# Run CoolBlazor Image
sudo docker run -itd -u root --ip 172.17.0.2 --privileged=true --restart=always -p 32796:5087 blazor-server-with-docker

# Run CoolWebApi Image
sudo docker run -itd -u root --ip 172.17.0.2 --privileged=true --restart=always -p 32798:5232 web-api-with-docker 
```

# Demonstrations
### Login and Register
1.Login without authorization.
![img](https://github.com/UMkashingHui/CoolBlazor/blob/master/CoolBlazor/CoolBlazor/wwwroot/images/Gifs/Login_NotActive.gif)
2.Register.
![img](https://github.com/UMkashingHui/CoolBlazor/blob/master/CoolBlazor/CoolBlazor/wwwroot/images/Gifs/Register.gif)
### Authorization(Make User Active!)
![img](https://github.com/UMkashingHui/CoolBlazor/blob/master/CoolBlazor/CoolBlazor/wwwroot/images/Gifs/ActivateUser.gif)
### Modify Personal Information (Crop Image as Avator)
![img](https://github.com/UMkashingHui/CoolBlazor/blob/master/CoolBlazor/CoolBlazor/wwwroot/images/Gifs/CropImageAsAvatar.gif)
### User Roles Control
Coming up.
### Roles Control
Coming up.
