FROM postgres:13-alpine
COPY ./example/Example/example.sql /docker-entrypoint-initdb.d/
