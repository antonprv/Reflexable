# Zenjex — Zenject-like API for Reflex

> **Familiar Zenject syntax. Modern Reflex performance. Works with the latest Unity versions.**

---

## 🇬🇧 English

### The Problem

Zenject is widely praised in the Unity community — but it hasn't kept up with modern Unity versions. Projects that rely on it face compatibility issues, abandoned support, and a framework that simply doesn't move forward.

**Reflex** is the answer: it's the most actively maintained and performant DI framework for Unity today. But switching from Zenject means rewriting your entire installer layer and retraining your team.

### The Solution

**Zenjex** is a thin extension layer on top of [Reflex 14.1.0](https://github.com/gustavopsantos/reflex) that brings a Zenject-familiar API to Reflex's modern engine. You keep the syntax your team already knows. You get all the benefits of Reflex under the hood.

On top of that, Zenjex solves a real Reflex limitation: **you can now add bindings to a container even after it has already been built** — a capability the base Reflex framework does not provide.

---

### Features

- **Zenject-style API** — `Bind<T>().To<TImpl>().AsSingle()` works exactly as you'd expect
- **Post-build container registration** — inject new bindings into an existing `Container` instance via `container.Bind<T>().FromInstance(...).AsSingle()`
- **`BindInterfaces()` / `BindInterfacesAndSelf()`** — automatic interface resolution, same as Zenject
- **`AsSingle()` / `AsTransient()` / `AsScoped()` / `AsEagerSingleton()`** — full lifetime control
- **`ProjectRootInstaller`** — a MonoBehaviour base class for global DI setup with lifecycle hooks
- **`RootContext`** — a static access point for resolving from the root container (for GameInstance-style architectures)
- **Built on Reflex 14.1.0** — full IL2CPP support, source generators, scoped containers

---

### Project Structure

```
src/
├── Reflex/              ← Reflex 14.1.0 (unchanged)
└── ReflexExtensions/    ← Zenjex extension layer
    ├── BindingBuilder.cs              ← Fluent API for ContainerBuilder (setup phase)
    ├── ContainerBindingBuilder.cs     ← Fluent API for Container (post-build registration)
    ├── ReflexZenjectExtensions.cs     ← Bind<T>() extension on ContainerBuilder
    ├── ContainerZenjectExtensions.cs  ← Bind<T>() extension on built Container
    ├── ProjectRootInstaller.cs        ← Base MonoBehaviour for global DI
    └── RootContext.cs                 ← Static resolver for GameInstance pattern
```

---

### Installation

1. Copy the `Reflex` folder into your Unity project
2. Copy the `ReflexExtensions` folder anywhere in your project

Then follow the standard Reflex setup from the [official Reflex repository](https://github.com/gustavopsantos/reflex) (create a `ProjectScope`, configure scene scopes, etc).

> **Note:** The TreeView debugger window has a known upstream bug in Reflex — the editor debug panel may behave incorrectly. This is a Reflex issue, not a Zenjex one.

---

### Usage

#### 1. Setting up bindings (ContainerBuilder)

```csharp
public class GameInstaller : ProjectRootInstaller
{
    public override void InstallBindings(ContainerBuilder builder)
    {
        // Bind interface to implementation, singleton
        builder.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();

        // Bind with all interfaces of the concrete type
        builder.Bind<PlayerProvider>().BindInterfaces().AsSingle();

        // Bind with interfaces AND the concrete type itself
        builder.Bind<PlayerProvider>().BindInterfacesAndSelf().AsSingle();

        // Transient (new instance on each resolve)
        builder.Bind<IEnemyFactory>().To<EnemyFactory>().AsTransient();

        // Eager singleton (instantiated immediately at build time)
        builder.Bind<IEventBus>().To<EventBus>().AsEagerSingleton();

        // From existing instance
        builder.Bind<ICoroutineRunner>().FromInstance(_myMonoBehaviour).AsSingle();

        // Platform-based conditional binding
        if (Application.platform != RuntimePlatform.Android)
            builder.Bind<IInputService>().To<PCInputService>().AsSingle();
        else
            builder.Bind<IInputService>().To<PhoneInputService>().AsSingle();
    }
}
```

#### 2. Post-build registration (on existing Container)

This is unique to Zenjex — Reflex doesn't support this natively.

```csharp
// GameInstance is created asynchronously AFTER the container is built
public override IEnumerator InstallGameInstanceRoutine()
{
    yield return InstallerFactory.CreateGameInstanceRoutine(instance =>
        _gameInstance = instance);

    // Add GameInstance to the already-built container
    RootContainer.Bind<GameInstance>()
        .FromInstance(_gameInstance)
        .BindInterfacesAndSelf()
        .AsSingle();
}
```

#### 3. ProjectRootInstaller

```csharp
public class GameInstaller : ProjectRootInstaller
{
    private GameInstance _gameInstance;

    // Step 1: Register all services into ContainerBuilder
    public override void InstallBindings(ContainerBuilder builder) { ... }

    // Step 2: Async routine — create late objects, add them to the built container
    public override IEnumerator InstallGameInstanceRoutine()
    {
        yield return InstallerFactory.CreateGameInstanceRoutine(i => _gameInstance = i);
        RootContainer.Bind<GameInstance>().FromInstance(_gameInstance).BindInterfacesAndSelf().AsSingle();
    }

    // Step 3: All bindings done — start the game
    public override void LaunchGame() => _gameInstance.LaunchGame();
}
```

#### 4. RootContext — resolving without injection

For cases where a class cannot receive dependencies through a constructor or `[Inject]` (e.g. a GameInstance singleton that needs services after DI is complete):

```csharp
private void ResolveDependencies()
{
    _staticData = RootContext.Resolve<IStaticDataService>();
}

// Guard check:
if (RootContext.HasInstance)
    var service = RootContext.Resolve<IMyService>();
```

---

### Binding Lifetime Reference

| Method | Lifetime | Notes |
|---|---|---|
| `AsSingle()` | Singleton | Alias for `AsSingleton()` |
| `AsSingleton()` | Singleton | One instance for the container's lifetime |
| `AsTransient()` | Transient | New instance on every resolve |
| `AsScoped()` | Scoped | One instance per scope |
| `AsEagerSingleton()` | Singleton (Eager) | Instantiated immediately when the container is built |

---

### Key Differences from Pure Reflex

| Feature | Pure Reflex | Zenjex |
|---|---|---|
| Fluent binding API | `builder.AddSingleton<T>()` | `builder.Bind<T>().To<TImpl>().AsSingle()` |
| Post-build registration | ❌ Not supported | ✅ `container.Bind<T>().FromInstance(x).AsSingle()` |
| Interface auto-binding | Manual | `BindInterfaces()` / `BindInterfacesAndSelf()` |
| GameInstance pattern | Requires custom setup | Built-in via `ProjectRootInstaller` + `RootContext` |

---

### Requirements

- Unity 2022.3+ (LTS) or newer
- Reflex 14.1.0 (included)
- .NET Standard 2.1

---

### License

© 2026 Anton Piruev. Any direct commercial use of derivative work is strictly prohibited. See [LICENSE](./LICENSE).

---
---

## 🇷🇺 Русский

### Проблема

Zenject долгое время был стандартом DI в Unity-проектах. Но он не совместим с актуальными версиями Unity, поддержка заброшена, а сообщество давно ищет замену.

**Reflex** — самый активно развивающийся и производительный DI-фреймворк для Unity на сегодняшний день. Но переход с Zenject означал полный переписывание всего installer-слоя и переобучение команды.

### Решение

**Zenjex** — это тонкий слой расширений поверх [Reflex 14.1.0](https://github.com/gustavopsantos/reflex), который добавляет привычный Zenject-like API. Команда продолжает писать так, как привыкла. Под капотом — современный Reflex.

Кроме того, Zenjex решает реальное ограничение Reflex: **можно добавлять биндинги в контейнер даже после того, как он был создан** (`Build()`) — чего базовый Reflex не умеет.

---

### Возможности

- **Zenject-style API** — `Bind<T>().To<TImpl>().AsSingle()` работает именно так, как вы ожидаете
- **Регистрация после Build()** — добавляйте биндинги в существующий `Container` через `container.Bind<T>().FromInstance(...).AsSingle()`
- **`BindInterfaces()` / `BindInterfacesAndSelf()`** — автоматическое связывание по интерфейсам, как в Zenject
- **`AsSingle()` / `AsTransient()` / `AsScoped()` / `AsEagerSingleton()`** — полный контроль над временем жизни
- **`ProjectRootInstaller`** — базовый MonoBehaviour для глобального DI с lifecycle-хуками
- **`RootContext`** — статическая точка доступа к корневому контейнеру (для архитектур с GameInstance-синглтоном)
- **Основан на Reflex 14.1.0** — полная поддержка IL2CPP, source generators, scoped-контейнеры

---

### Структура проекта

```
src/
├── Reflex/              ← Reflex 14.1.0 (без изменений)
└── ReflexExtensions/    ← расширения Zenjex
    ├── BindingBuilder.cs              ← Fluent API для ContainerBuilder (фаза настройки)
    ├── ContainerBindingBuilder.cs     ← Fluent API для Container (регистрация после Build)
    ├── ReflexZenjectExtensions.cs     ← Bind<T>() extension-метод на ContainerBuilder
    ├── ContainerZenjectExtensions.cs  ← Bind<T>() extension-метод на готовом Container
    ├── ProjectRootInstaller.cs        ← Базовый MonoBehaviour для глобального DI
    └── RootContext.cs                 ← Статический резолвер для паттерна GameInstance
```

---

### Установка

1. Скопируйте папку `Reflex` в ваш Unity-проект
2. Скопируйте папку `ReflexExtensions` в любое место проекта

Далее следуйте стандартной настройке Reflex из [официального репозитория](https://github.com/gustavopsantos/reflex) (создайте `ProjectScope`, настройте scene scopes и т.д.).

> **Известный баг:** TreeView-окно дебаггера в Reflex недоработано автором — редакторская панель отладки может работать некорректно. Это upstream-проблема Reflex, не Zenjex.

---

### Использование

#### 1. Настройка биндингов (ContainerBuilder)

```csharp
public class GameInstaller : ProjectRootInstaller
{
    public override void InstallBindings(ContainerBuilder builder)
    {
        // Интерфейс → реализация, синглтон
        builder.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();

        // Привязать по всем интерфейсам конкретного типа
        builder.Bind<PlayerProvider>().BindInterfaces().AsSingle();

        // Привязать по интерфейсам И по самому конкретному типу
        builder.Bind<PlayerProvider>().BindInterfacesAndSelf().AsSingle();

        // Transient — новый экземпляр при каждом резолве
        builder.Bind<IEnemyFactory>().To<EnemyFactory>().AsTransient();

        // Eager singleton — создаётся сразу при Build()
        builder.Bind<IEventBus>().To<EventBus>().AsEagerSingleton();

        // Из готового экземпляра
        builder.Bind<ICoroutineRunner>().FromInstance(_myMonoBehaviour).AsSingle();

        // Условный биндинг по платформе
        if (Application.platform != RuntimePlatform.Android)
            builder.Bind<IInputService>().To<PCInputService>().AsSingle();
        else
            builder.Bind<IInputService>().To<PhoneInputService>().AsSingle();
    }
}
```

#### 2. Регистрация после Build() (на существующем Container)

Уникальная возможность Zenjex — в чистом Reflex это недоступно.

```csharp
// GameInstance создаётся асинхронно ПОСЛЕ того, как контейнер уже построен
public override IEnumerator InstallGameInstanceRoutine()
{
    yield return InstallerFactory.CreateGameInstanceRoutine(instance =>
        _gameInstance = instance);

    // Добавляем GameInstance в уже построенный контейнер
    RootContainer.Bind<GameInstance>()
        .FromInstance(_gameInstance)
        .BindInterfacesAndSelf()
        .AsSingle();
}
```

#### 3. ProjectRootInstaller

```csharp
public class GameInstaller : ProjectRootInstaller
{
    private GameInstance _gameInstance;

    // Шаг 1: Регистрируем все сервисы в ContainerBuilder
    public override void InstallBindings(ContainerBuilder builder) { ... }

    // Шаг 2: Асинхронная рутина — создаём объекты с задержкой, добавляем в контейнер
    public override IEnumerator InstallGameInstanceRoutine()
    {
        yield return InstallerFactory.CreateGameInstanceRoutine(i => _gameInstance = i);
        RootContainer.Bind<GameInstance>().FromInstance(_gameInstance).BindInterfacesAndSelf().AsSingle();
    }

    // Шаг 3: Всё готово — запускаем игру
    public override void LaunchGame() => _gameInstance.LaunchGame();
}
```

#### 4. RootContext — резолв без инъекции

Для случаев, когда класс не может получить зависимость через конструктор или `[Inject]` (например, GameInstance-синглтон, которому нужны сервисы уже после завершения DI):

```csharp
private void ResolveDependencies()
{
    _staticData = RootContext.Resolve<IStaticDataService>();
}

// Проверка перед использованием:
if (RootContext.HasInstance)
    var service = RootContext.Resolve<IMyService>();
```

---

### Справочник по времени жизни

| Метод | Время жизни | Примечание |
|---|---|---|
| `AsSingle()` | Singleton | Псевдоним для `AsSingleton()` |
| `AsSingleton()` | Singleton | Один экземпляр на весь контейнер |
| `AsTransient()` | Transient | Новый экземпляр при каждом резолве |
| `AsScoped()` | Scoped | Один экземпляр на scope |
| `AsEagerSingleton()` | Singleton (Eager) | Создаётся сразу при `Build()` |

---

### Ключевые отличия от чистого Reflex

| Возможность | Чистый Reflex | Zenjex |
|---|---|---|
| Fluent API биндинга | `builder.AddSingleton<T>()` | `builder.Bind<T>().To<TImpl>().AsSingle()` |
| Регистрация после Build() | ❌ Не поддерживается | ✅ `container.Bind<T>().FromInstance(x).AsSingle()` |
| Автопривязка по интерфейсам | Вручную | `BindInterfaces()` / `BindInterfacesAndSelf()` |
| Паттерн GameInstance | Требует ручной реализации | Встроен: `ProjectRootInstaller` + `RootContext` |

---

### Требования

- Unity 2022.3+ (LTS) или новее
- Reflex 14.1.0 (включён в репозиторий)
- .NET Standard 2.1

---

### Лицензия

© 2026 Anton Piruev. Прямое коммерческое использование производных работ строго запрещено. См. [LICENSE](./LICENSE).
