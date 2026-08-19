# Revenda - Serviço de Identidade

API responsável pelo cadastro e pela autenticação dos compradores da plataforma de
revenda de veículos. É o serviço que o enunciado exige manter apartado dos dados
transacionais: nenhum dado pessoal sai daqui, apenas o token que identifica quem está
comprando.

Serviço irmão: `revenda-vehicles`, que cuida do estoque e das vendas e valida os tokens
emitidos por este serviço.

## O que ele faz

| Método | Rota | Acesso | Descrição |
| --- | --- | --- | --- |
| POST | `/customers` | público | Cadastra o comprador |
| POST | `/auth/login` | público | Autentica e devolve o token de acesso |
| GET | `/customers/me` | autenticado | Dados do próprio cadastro |
| PUT | `/customers/me` | autenticado | Atualiza nome e e-mail |
| GET | `/.well-known/jwks.json` | público | Chave pública de assinatura do token |
| GET | `/.well-known/openid-configuration` | público | Metadados para validação automática |
| GET | `/health` | público | Verificação de saúde da API e do banco |

A documentação interativa fica em `/swagger`.

## Como foi implementado

Arquitetura hexagonal com a regra de dependência da Clean Architecture. Quatro projetos:

- `Domain`: `Customer` e os objetos de valor `Cpf`, `Email` e `Password`. Não conhece
  banco, HTTP nem framework. A entidade só existe em estado válido.
- `Application`: casos de uso (cadastro, consulta, atualização e autenticação) e as
  portas de entrada e saída. Nenhuma referência a EF Core ou ASP.NET.
- `Infrastructure`: adaptadores de saída. EF Core com PostgreSQL, hash PBKDF2 e emissão
  de JWT assinado em RS256.
- `Api`: adaptador de entrada. Controllers, tradução de exceção para `ProblemDetails`,
  autenticação e Swagger.

O token carrega apenas `sub` (identificador do comprador) e `role`. Nome, CPF e e-mail
não entram no token justamente para não vazarem para o serviço de vendas.

A chave de assinatura é RSA. A privada fica só aqui; a pública é publicada em
`/.well-known/jwks.json`, e é assim que o serviço de veículos valida o token sem que
exista qualquer segredo compartilhado entre os dois.

## Rodando localmente

Com Docker, que é o caminho mais curto:

```bash
cp .env.example .env
docker compose up -d --build
```

A API sobe em `http://localhost:8081` e o PostgreSQL em `localhost:5433`. As migrations
são aplicadas na subida e o administrador definido no `.env` é criado se ainda não
existir.

Sem Docker, com o PostgreSQL já disponível:

```bash
dotnet restore
dotnet tool restore
dotnet run --project src/Revenda.Identity.Api
```

A string de conexão padrão está em `src/Revenda.Identity.Api/appsettings.json` e pode ser
sobrescrita por `ConnectionStrings__Postgres`.

### Chave de assinatura

Em desenvolvimento, se `Jwt__PrivateKeyPem` não for informada, o serviço gera uma chave
efêmera e registra um aviso no log — os tokens deixam de valer a cada reinício. Para um
ambiente estável, gere a chave e configure a variável:

```bash
openssl genrsa -out identity.pem 2048
```

## Testando

```bash
dotnet test
```

Os testes unitários cobrem o domínio e os casos de uso e rodam sem dependência externa.
Os de integração sobem um PostgreSQL real via Testcontainers, então precisam do Docker
em execução.

Teste manual do fluxo completo:

```bash
curl -X POST http://localhost:8081/customers \
  -H "Content-Type: application/json" \
  -d '{"name":"Ana Silva","cpf":"529.982.247-25","email":"ana@revenda.com","password":"Revenda2026"}'

curl -X POST http://localhost:8081/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"ana@revenda.com","password":"Revenda2026"}'

curl http://localhost:8081/customers/me -H "Authorization: Bearer <token>"
```

## Entrega contínua

`ci.yml` roda a cada Pull Request: restore, build com warnings tratados como erro, testes
unitários e de integração, e publicação do relatório de cobertura como artefato.

`cd.yml` roda no merge para `main`: publica a imagem no GHCR com as tags `latest` e
`sha-<commit>` e atualiza a stack no host configurado nas variáveis `DEPLOY_HOST`,
`DEPLOY_USER` e `DEPLOY_PATH`, com a chave em `DEPLOY_SSH_KEY`.

A branch `main` é protegida: alterações entram apenas por Pull Request com o CI verde.
