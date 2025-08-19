ARG APP_VERSION=0.0.1
ARG APP_SRC_REVISION='NotSet'
ARG DOTNET_SDK_VERSION='NotSet'
ARG DOTNET_RUNTIME_VERSION='NotSet'

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build
ARG DOTNET_SDK_VERSION
ARG DOTNET_RUNTIME_VERSION
ARG APP_VERSION
ARG APP_SRC_REVISION
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /src

RUN --mount=type=bind,target=/src,source=./,ro \
    --mount=type=tmpfs,target=/tmp \
    --mount=type=cache,target=/root/.dotnet,sharing=locked \
    --mount=type=cache,target=/root/.nuget,sharing=locked \
    --mount=type=cache,target=/root/.local/share/NuGet,sharing=locked \
    --mount=type=cache,target=/root/.local/share/NuGet/Cache,sharing=locked \
    dotnet tool restore

RUN --mount=type=bind,target=/src,source=./,rw \
    --mount=type=tmpfs,target=/tmp \
    --mount=type=cache,target=/root/.dotnet,sharing=locked \
    --mount=type=cache,target=/root/.nuget,sharing=locked \
    --mount=type=cache,target=/root/.local/share/NuGet,sharing=locked \
    --mount=type=cache,target=/root/.local/share/NuGet/Cache,sharing=locked \
    dotnet paket restore --silent --fail-on-checks

RUN --mount=type=bind,target=/src,source=./,rw \
    --mount=type=tmpfs,target=/tmp \
    --mount=type=cache,target=/root/.dotnet,sharing=locked \
    --mount=type=cache,target=/root/.nuget,sharing=locked \
    --mount=type=cache,target=/root/.local/share/NuGet,sharing=locked \
    --mount=type=cache,target=/root/.local/share/NuGet/Cache,sharing=locked \
    dotnet publish Concepts.Crud.WebApi/Concepts.Crud.WebApi.csproj --nologo --no-self-contained --output=/app --configuration Release --framework="net${DOTNET_RUNTIME_VERSION}" -p:RuntimeIdentifiers="linux-x64" -p:VersionPrefix=${APP_VERSION} -p:SourceRevisionId=${APP_SRC_REVISION} -p:PaketDisableGlobalRestore=true

# final clenup
SHELL [ "/usr/bin/bash", "-c" ]
RUN rm -rfv /app/runtimes/{browser,linux-arm*,linux-musl*,osx*,win*} \
 && rm -rfv /app/{cs,de,es,fr,it,ja,ko,pl,pt-BR,ru,tr,zh-Hans,zh-Hant} \
 && rm -f /app/*.config /app/*.cfg

# final rootfs-overlay files and permissions
RUN mkdir -p /rootfs/opt/crud \
 && mv /app/* /rootfs/opt/crud/
COPY --link ./Concepts.Crud.WebApi/nlog.config ./Concepts.Crud.WebApi/appconfig.cfg /rootfs/etc/crud/
RUN find /rootfs -type f -exec chmod 400 {} + \
 && chmod u+x /rootfs/opt/crud/Concepts.Crud.WebApi

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_RUNTIME_VERSION}
ARG DOTNET_RUNTIME_VERSION
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

COPY --link --from=build --chown=${APP_UID}:${APP_UID} /rootfs/ /

USER $APP_UID
ENTRYPOINT ["/opt/crud/Concepts.Crud.WebApi"]
CMD ["--logconfig", "/etc/crud/nlog.config", "--config", "/etc/crud/appconfig.cfg"]
