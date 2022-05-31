CREATE TABLE IF NOT EXISTS $schema.$table (
    app varchar(100) not null,
    env varchar(100) not null default '*',
    host varchar(100) not null default '*',
    key varchar(500) not null,
    value varchar not null,
    CONSTRAINT $table_pk PRIMARY KEY (app, env, host, key)
);

COMMENT ON TABLE $schema.$table IS 'source for Microsoft.Extensions.Configuration.Npgsql configuration provider';
