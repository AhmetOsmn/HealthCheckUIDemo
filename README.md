# HealthCheckUIDemo

- Birbirleri ile etkileşim içersinde olan component'ların durmularını kontrol eden örnek proje.

# Notlar

- Test servislerini solution içerisinden veya dotnet cli komutlarını kullanarak çalıştırabilir ve health check ekranından takip edebilirsiniz.

# Docker Container'larının Takibi

- Uygulama içerisinde alt kısımdaki contaner'ların durumları takip edilmektedir:
  - Elastic Search
  - Consul
  - Rabbit MQ
  - Redis
  - Postgres Database
  - Sql Server Database

# Servislerin Takibi

- Uygulama içerisinde solution içerisinde bulunan alt kısımdaki servislerin durumları takip edilmektedir:
  - HealthCheckUIDemo.TestAPI
  - HealthCheckUIDemo.TestMVC


# Görseller

![image](https://github.com/user-attachments/assets/1e2edede-76b6-4cab-b5bd-c3e7ca2223ba)


# Kaynaklar

- [Microsoft](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health)
- [Burak Selim Senyurt - Distributed Challenge](https://github.com/buraksenyurt/DistributedChallenge)
