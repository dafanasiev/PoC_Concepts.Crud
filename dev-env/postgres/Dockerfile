ARG IMAGE_VERSION=17.5
FROM postgres:${IMAGE_VERSION}

ENV DEBIAN_FRONTEND noninteractive

RUN true && \
    echo "ru_RU.UTF-8 UTF-8" >> /etc/locale.gen && \
    echo "en_US.UTF-8 UTF-8" >> /etc/locale.gen && \
    locale-gen
