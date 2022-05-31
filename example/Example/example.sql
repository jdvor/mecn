CREATE SCHEMA IF NOT EXISTS example;

CREATE TABLE IF NOT EXISTS example.appsettings (
    app varchar(100) not null,
    env varchar(100) not null default '*',
    host varchar(100) not null default '*',
    key varchar(500) not null,
    value varchar not null,
    CONSTRAINT appsettings_pk PRIMARY KEY (app, env, host, key)
);


INSERT INTO example.appsettings (app, env, host, key, value) VALUES ('example', '*', '*', 'key1', 'val1')
    ON CONFLICT ON CONSTRAINT appsettings_pk DO UPDATE SET value = 'val1';

INSERT INTO example.appsettings (app, env, host, key, value) VALUES ('example', 'stage', '*', 'root:key2', 'val2')
    ON CONFLICT ON CONSTRAINT appsettings_pk DO UPDATE SET value = 'val2';

INSERT INTO example.appsettings (app, env, host, key, value) VALUES ('example', 'prod', '*', 'root:key2', 'val3')
    ON CONFLICT ON CONSTRAINT appsettings_pk DO UPDATE SET value = 'val3';

INSERT INTO example.appsettings (app, env, host, key, value) VALUES ('example', 'stage', '*', 'root:key3', 'val4')
    ON CONFLICT ON CONSTRAINT appsettings_pk DO UPDATE SET value = 'val4';

INSERT INTO example.appsettings (app, env, host, key, value) VALUES ('example', 'stage', 'example.com', 'root:key3', 'val5')
    ON CONFLICT ON CONSTRAINT appsettings_pk DO UPDATE SET value = 'val5';

INSERT INTO example.appsettings (app, env, host, key, value) VALUES ('example', 'prod', '*', 'root:key3', 'val6')
    ON CONFLICT ON CONSTRAINT appsettings_pk DO UPDATE SET value = 'val6';

INSERT INTO example.appsettings (app, env, host, key, value) VALUES ('example', 'prod', 'example.com', 'root:key3', 'val7')
    ON CONFLICT ON CONSTRAINT appsettings_pk DO UPDATE SET value = 'val7';

INSERT INTO example.appsettings (app, env, host, key, value) VALUES ('app2', 'stage', '*', 'root:key4', 'val8')
    ON CONFLICT ON CONSTRAINT appsettings_pk DO UPDATE SET value = 'val8';

INSERT INTO example.appsettings (app, env, host, key, value) VALUES ('app2', 'prod', '*', 'root:key4', 'val9')
    ON CONFLICT ON CONSTRAINT appsettings_pk DO UPDATE SET value = 'val9';
