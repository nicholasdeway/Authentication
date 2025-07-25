# Autenticação de Usuário com JWT e MongoDB

Este projeto implementa um sistema de autenticação seguro usando **JWT (JSON Web Token)** e **MongoDB**.  
Inclui funcionalidades de registro, login e proteção de rotas com middleware de autorização.

---

## Tecnologias Utilizadas
- C# / .NET 8
- ASP.NET Core Web API
- MongoDB
- JWT (Json Web Token)

---

## Dependências do projeto

- BCrypt.Net-Next (4.0.3)
- Microsoft.AspNetCore.Authentication.JwtBearer (8.0.0 / 9.0.7)
- MongoDB.Driver (3.4.0 / 3.4.2)
- Swashbuckle.AspNetCore (6.6.2 / 9.0.3)
- System.IdentityModel.Tokens.Jwt (8.12.1 / 8.13.0)

---

## Funcionalidades
- Registro de usuários
- Login com geração de token JWT
- Middleware para validação de token e autorização
- Armazenamento seguro de senhas (hash)
- Persistência de dados no MongoDB

---

## Pré-requisitos
Antes de rodar o projeto, você precisa ter instalado:
- [MongoDB](https://www.mongodb.com/try/download/community-kubernetes-operator)
- [Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/downloads/)

## Configurações no appsettings.Development
- Use no terminal para gerar a key JWT: **node -e "console.log(require('crypto').randomBytes(256).toString('base64'))"**
