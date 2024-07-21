### Plann.er API

O projeto **Plann.er** tem como objetivo ajudar o usuário a organizar viagens à trabalho ou lazer. Com essa API, você pode criar viagens e planejar atividades diárias.

#### Funcionalidades

- Criar e gerenciar viagens.
- Adicionar e organizar atividades diárias para cada viagem.

### Endpoints Principais

#### Viagens
- **Criar viagem**: `POST api/trips`
- **Obter detalhes da viagem**: `GET /api/trips/{tripId}`
- **Atualizar viagem**: `PUT /api/trips/{tripId}`
- **Deletar viagem**: `DELETE /api/trips/{tripId}`

#### Atividades
- **Adicionar atividade**: `POST /api/trips/{tripId}/activity`
- **Completar atividade**: `PUT /api/trips/{tripId}/activity/{activityId}/complete`
- **Deletar atividade**: `POST /api/trips/{tripId}/activity/{activityId}`
- **Obter atividades**: `GET /api/trips/{tripId}/activities`

### Rodando a API localmente

#### Pré-requisitos

- .NET SDK 8.0 ou superior
- Visual Studio ou VS Code

#### Passos

1. Clone o repositório:
    ```bash
    git clone https://github.com/dffs-code/plann.er.git
    cd plann.er
    ```

2. Restaure os pacotes NuGet:
    ```bash
    dotnet restore
    ```

3. Inicie a aplicação:
    ```bash
    dotnet run
    ```

4. A API estará disponível em `https://localhost:5001`.

### Especificações da API

A API segue as especificações OpenAPI 3.0, com a documentação disponível na rota `/swagger` após iniciar a aplicação.

### Banco de Dados

O banco de dados utilizado é o SQLite e sua estrutura é composta pelas seguintes tabelas:

```sql
CREATE TABLE "Activities" (
	"Id"	TEXT NOT NULL,
	"Name"	TEXT NOT NULL,
	"Date"	TEXT NOT NULL,
	"Status"	INTEGER NOT NULL DEFAULT 0,
	"TripId"	TEXT NOT NULL,
	PRIMARY KEY("Id"),
	FOREIGN KEY("TripId") REFERENCES Trips(Id) ON DELETE CASCADE
)

CREATE TABLE "Trips" (
	"Id"	TEXT NOT NULL,
	"Name"	TEXT NOT NULL,
	"StartDate"	TEXT NOT NULL,
	"EndDate"	TEXT NOT NULL,
	"UserId"	TEXT NOT NULL,
	"Country"	TEXT NOT NULL,
	"City"	TEXT NOT NULL,
	"FullAddress"	TEXT NOT NULL,
	PRIMARY KEY("Id"),
	FOREIGN KEY("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
)

CREATE TABLE "Users" (
	"Id"	TEXT NOT NULL,
	"Username"	TEXT NOT NULL,
	"Password"	TEXT NOT NULL,
	"Salt"	TEXT NOT NULL,
	"Email"	TEXT NOT NULL,
	PRIMARY KEY("Id")
)
```

### Contribuindo

Se você deseja contribuir com este projeto, sinta-se à vontade para abrir uma issue ou enviar um pull request no repositório do GitHub.

### Licença

Este projeto está licenciado sob a [MIT License](LICENSE).
