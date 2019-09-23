FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
 

ARG Version=0.0.0
RUN echo "Version=$Version"  
 
# copy and publish app and libraries
COPY ./ ./
RUN dotnet restore
RUN find -type d -name bin -prune -exec rm -rf {} \; && find -type d -name obj -prune -exec rm -rf {} \;

RUN dotnet restore

RUN dotnet build   -c Release --no-restore -p:Version=$Version
RUN dotnet test   -c Release --no-restore -p:Version=$Version
RUN dotnet pack    -c Release --no-restore --no-build -p:Version=$Version -o "./dist/pack"