FROM mcr.microsoft.com/mssql/server

COPY ./DataBase /usr/src/app/
COPY ../script.sql /usr/src/app/
USER root
RUN chmod 755 /usr/src/app/entrypoint.sh
USER mssql
ENTRYPOINT ["/usr/src/app/entrypoint.sh"]

CMD [ "/opt/mssql/bin/sqlservr" ]