GIT_HASH=$(shell git rev-parse --short HEAD)
CI_BUILD_REVISION ?= $(GIT_HASH)

BUILD_DATE=$(shell date -u '+%Y-%m-%d %I:%M:%S %Z')

CI_BUILD_NUMBER ?= 0
CI_BUILD_BRANCH ?= master

VER_MAJOR=1
VER_MINOR=0
VER_PATCH=$(CI_BUILD_NUMBER)
VERSION = $(VER_MAJOR).$(VER_MINOR).$(VER_PATCH)

all: oci-image

oci-image:
# конечно совсем не верно считать что DOTNET_SDK_VERSION содержит первые два числа равной DOTNET_RUNTIME_VERSION, но вроде как сойдёт
	docker buildx build \
                --build-arg APP_VERSION=$(VERSION) \
                --build-arg APP_SRC_REVISION=$(CI_BUILD_REVISION) \
                --build-arg DOTNET_SDK_VERSION=$(shell grep dotnet ../.tool-versions | awk '{print $$2}') \
                --build-arg DOTNET_RUNTIME_VERSION=$(shell grep dotnet ../.tool-versions | awk '{print $$2}' | grep -oP '\d+\.\d+') \
                --progress=plain --pull \
                -t concepts/crud:$(VERSION) \
                -t concepts/crud:latest .

oci-image-push:
	docker image push concepts/crud:$(VERSION)
	docker image push concepts/crud:latest
