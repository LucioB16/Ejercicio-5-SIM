# Simulación CMI

Este repositorio contiene una implementación en C# 12 y .NET 8 del ejercicio "CMI Corporation". El objetivo es estimar mediante simulación la efectividad de un anuncio televisivo y calcular cuántas visitas se requieren para alcanzar un número determinado de ventas.

## Enunciado

CMI Corporation llevó a cabo una prueba, diseñada para evaluar la efectividad de un nuevo anuncio por televisión para uno de sus productos domésticos. El anuncio se mostró en un mercado de prueba durante dos semanas. En el estudio de seguimiento se contactó telefónicamente con una selección aleatoria de personas y se les hizo una serie de preguntas sobre la posible compra del producto. Se obtuvieron:

### Probabilidades a priori

| Evento | Probabilidad |
| --- | ---: |
| El individuo recordaba el mensaje | 0,35 |
| El individuo no podía recordar el mensaje | 0,65 |

### Probabilidades condicionales

| | Definitivamente no | Dudoso | Definitivamente sí |
| --- | ---: | ---: | ---: |
| Podía recordar el mensaje | 0,55 | 0,15 | 0,30 |
| No podía recordar el mensaje | 0,70 | 0,25 | 0,05 |

Objetivos:

1. Estimar por simulación la probabilidad general de que un individuo responda "definitivamente sí".
2. Si el 60 % de los que respondieron "dudoso" terminan comprando, determinar cuántas visitas se necesitan para vender 10 000 productos.

## Ejecución de pruebas

Para ejecutar las pruebas unitarias del motor de simulación:

```bash
dotnet test SimulacionCmi.Core.Tests/SimulacionCmi.Core.Tests.csproj
```

## Estructura

- `SimulacionCmi.Core`: biblioteca con el motor de simulación y la clase `VectorEstado`.
- `SimulacionCmi`: aplicación WPF (solo compilable en Windows) que consume la biblioteca.
- `SimulacionCmi.Core.Tests`: pruebas automatizadas con xUnit.

## Licencia

MIT
