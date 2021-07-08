﻿CREATE TABLE public.avisos
(
    id            INT8 NOT NULL GENERATED ALWAYS AS identity,
    texto         VARCHAR(255) NOT NULL,
    usuario_id    INT8         NOT NULL,
    curso_id      INT8         NOT NULL,
    data_inclusao TIMESTAMP    NOT NULL,
    data_alteracao TIMESTAMP   NULL
);

ALTER TABLE public.avisos
    ADD CONSTRAINT mural_pk
        PRIMARY KEY (id);

ALTER TABLE public.avisos
    ADD CONSTRAINT mural_curso_fk
        FOREIGN KEY (curso_id)
            REFERENCES cursos (id);

ALTER TABLE public.avisos
    ADD CONSTRAINT mural_usuario_fk
        FOREIGN KEY (usuario_id)
            REFERENCES usuarios (indice);

create index mural_curso_id_idx
    on avisos (curso_id);

create index mural_usuario_id_idx
    on avisos (usuario_id);