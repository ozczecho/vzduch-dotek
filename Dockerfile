FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /source

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app --no-restore -p:PublishTrimmed=false VzduchDotek.Net.csproj

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

LABEL Mike=ozczecho@yahoo.com

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl locales \
    && rm -rf /var/lib/apt/lists/*

RUN sed -i -e 's/# en_AU.UTF-8 UTF-8/en_AU.UTF-8 UTF-8/' /etc/locale.gen && \
    locale-gen
ENV TZ=Australia/Sydney
ENV LANG=en_AU.utf8
ENV LANGUAGE=${LANG}
ENV LC_ALL=${LANG}

WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["/app/VzduchDotek.Net"]
