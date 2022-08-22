FROM ubuntu:latest as build

ENV SOPS_VERSION="3.7.2"

RUN apt-get update && apt-get install wget -y
RUN mkdir /custom-tools
RUN wget -qO /custom-tools/sops https://github.com/mozilla/sops/releases/download/v${SOPS_VERSION}/sops-v${SOPS_VERSION}.linux
RUN chmod +x /custom-tools/*

FROM ubuntu:latest as base

ENV ARGOCD_ENV_SOPS_EXE="/custom-tools/sops"

COPY --from=build /custom-tools /custom-tools
WORKDIR /app

COPY . .
