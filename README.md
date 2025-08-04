# Simulación CMI

Este proyecto contiene una simulación del problema de la empresa **CMI Corporation** usando C# 12 y .NET 8.
La aplicación WPF permite configurar las probabilidades, ejecutar la simulación y
visualizar tablas y gráficos con los resultados.

## Parámetros de entrada

En la ventana principal se deben ingresar los siguientes valores:

| Parámetro | Rango válido |
| --- | --- |
| Visitas a simular | 10 &le; valor &le; 2 147 483 647 |
| Desde visita | 1 &le; valor &le; Hasta visita |
| Hasta visita | valor &le; Visitas a simular y (Hasta − Desde) &le; 10 000 |
| Semilla RNG | vacío para aleatoria, o cualquier entero de 32 bits |
| P(Recuerda) | 0 < valor < 1 |
| P(Compra | Dudoso) | 0 < valor < 1 |
| Ventas objetivo | 0 < valor < 2 147 483 647 |
| Tabla "Recuerda" | tres columnas, cada celda 0 < x < 1, suma de la fila = 1 |
| Tabla "No recuerda" | mismas reglas que la tabla "Recuerda" |

## Enunciado

CMI Corporation llevó a cabo una prueba para evaluar la efectividad de un nuevo anuncio por televisión para uno de sus productos domésticos. El anuncio se mostró en un mercado de prueba durante dos semanas. En el estudio de seguimiento se contactó telefónicamente con una selección aleatoria de personas y se les hizo una serie de preguntas sobre la posible compra del producto.

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

Los objetivos del estudio son:

1. Estimar por simulación la probabilidad general de que un individuo responda "definitivamente sí".
2. Si el 60 % de los que respondieron "dudoso" terminan comprando, determinar cuántas visitas se necesitan para vender 10 000 productos.

## Estructura

- `SimulacionCmi` – proyecto WPF (no se construye en Linux).
- `SimulacionCmiCore` – biblioteca con el motor de simulación.
- `SimulacionCmiCore.Tests` – pruebas unitarias xUnit.

## Compilación y pruebas

```bash
# compilar la biblioteca principal
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet build SimulacionCmiCore/SimulacionCmiCore.csproj

# ejecutar las pruebas unitarias
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet test SimulacionCmiCore.Tests/SimulacionCmiCore.Tests.csproj
```

## Licencia

Uso académico.
