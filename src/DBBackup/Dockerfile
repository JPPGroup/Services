FROM mcr.microsoft.com/powershell:latest
LABEL org.opencontainers.image.source https://github.com/JPPGroup/Services

RUN apt-get update &&\
apt-get install -y --no-install-recommends gnupg2 wget vim lsb-release curl

RUN echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list &&\
curl -fsSL https://www.postgresql.org/media/keys/ACCC4CF8.asc | gpg --dearmor -o /etc/apt/trusted.gpg.d/postgresql.gpg &&\
apt-get update && apt-get install -y --no-install-recommends \
postgresql-client-16 \
&& \
apt-get clean && \
rm -rf /var/lib/apt/lists/*

ADD https://aka.ms/downloadazcopy-v10-linux azcopy.tar.gz
RUN tar -xzvf azcopy.tar.gz --strip-components=1

COPY exec.ps1 exec.ps1

ENTRYPOINT ["pwsh", "exec.ps1"]