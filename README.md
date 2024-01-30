# Banco KRT

## Configurações do projeto
  - Para fazer os testes é preciso ter um banco DynamoDB criado na aws
  - Criar o banco DynamoDB com o nome "LimitManager"
  - No DynamoDB crie este Global Secondary Index (GSI)
    - ![image](https://github.com/vagnerkuntz/technical-challenge-btg-pactual/assets/30324117/d42ad3bd-c347-465a-8fbb-669ed99b0335)
  - Você precisa ter a extensão da AWS Toolkit instalada e com as chaves configuradas
  - arquivo appsettings.json troque para
  ```json
  "AWS": {
    "Profile": "COLOQUE-SEU-PROFILE-DO-AWS-TOOLKIT",
    "Region": "COLOQUE-A-REGIÃO-DA-AWS"
  }
  ```

## Para documentação e testes dos endpoints é usado o swagger
