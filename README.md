### Plann.er API

O projeto **Plann.er** tem como objetivo ajudar o usuário a organizar viagens à trabalho ou lazer. Com essa API, você pode criar viagens, planejar atividades diárias e gerenciar participantes.

#### Funcionalidades

- Criar e gerenciar viagens.
- Adicionar e organizar atividades diárias para cada viagem.
- Convidar e confirmar participantes para as viagens.
- Enviar convites por e-mail para participantes.

### Endpoints Principais

#### Viagens
- **Criar viagem**: `POST /trips`
- **Obter detalhes da viagem**: `GET /trips/{tripId}`
- **Atualizar viagem**: `PUT /trips/{tripId}`
- **Confirmar viagem**: `GET /trips/{tripId}/confirm`

#### Atividades
- **Adicionar atividade**: `POST /trips/{tripId}/activities`
- **Obter atividades**: `GET /trips/{tripId}/activities`

#### Participantes
- **Convidar participante**: `POST /trips/{tripId}/invites`
- **Confirmar participante**: `PATCH /participants/{participantId}/confirm`
- **Obter participantes**: `GET /trips/{tripId}/participants`

### Rodando a API localmente

#### Pré-requisitos

- .NET SDK 6.0 ou superior
- Visual Studio ou VS Code

#### Passos

1. Clone o repositório:
    ```bash
    git clone https://github.com/dffs-code/plann.er.git
    cd plann.er
    ```

2. Configure a string de conexão com o banco de dados no arquivo `appsettings.json`:
    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=plann-er.db"
    }
    ```

3. Restaure os pacotes NuGet:
    ```bash
    dotnet restore
    ```

4. Execute as migrações para criar o banco de dados:
    ```bash
    dotnet ef database update
    ```

5. Inicie a aplicação:
    ```bash
    dotnet run
    ```

6. A API estará disponível em `https://localhost:5001`.

### Especificações da API

A API segue as especificações OpenAPI 3.0, com a documentação disponível na rota `/swagger` após iniciar a aplicação.

### Contribuindo

Se você deseja contribuir com este projeto, sinta-se à vontade para abrir uma issue ou enviar um pull request no repositório do GitHub.

### Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

---

### Contato

Para mais informações, entre em contato através do e-mail: [contato@plannerapi.com](mailto:contato@plannerapi.com)

---

*Arquivo gerado automaticamente a partir das especificações da API no arquivo `planner.json`.*