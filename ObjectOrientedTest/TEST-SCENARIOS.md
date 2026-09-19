# Repository manager test scenarios

Build the project as a class library:

```bash
dotnet build ObjectOrientedTest.csproj
```

Run the unit tests:

```bash
dotnet test ObjectOrientedTest.sln
```

## Required API scenarios

| Scenario | Action | Expected result |
| --- | --- | --- |
| Constructor readiness | Create `RepositoryManager`; call `Initialize()` once | No exception; the repository is usable |
| JSON registration | Register `{"name":"Ada"}` with type `1`; call `Retrieve` and `GetType` | The exact JSON string is returned; type is `1` |
| XML registration | Register `<person><name>Ada</name></person>` with type `2`; call `Retrieve` and `GetType` | The exact XML string is returned; type is `2` |
| Invalid content | Register malformed JSON or XML for its type | `ArgumentException`; no item is stored |
| Unsupported type | Register with a type other than `1` or `2` | `ArgumentOutOfRangeException` |
| Duplicate name | Register the same name twice with different content | Second call throws `InvalidOperationException`; first content remains unchanged |
| Deregistration | Register an item, deregister it, then retrieve it | Retrieval throws `KeyNotFoundException` |
| Missing item | Retrieve, get the type of, or deregister an unknown name | `KeyNotFoundException` |
| Invalid name | Use `null`, empty, or whitespace for `itemName` | Argument exception |

## Optional challenge scenarios

- Run concurrent registrations using the same name. Exactly one registration succeeds; the others throw `InvalidOperationException`.
- Run concurrent reads and deregistration. No collection or synchronization exception occurs.
- Implement `IRepositoryStorage` with another backing store and pass it to the second `RepositoryManager` constructor.
- Add an `IItemContentValidator` for a new `RepositoryItemType` without storing values as `object`.

`Initialize()` is intentionally empty because construction creates the in-memory storage and validator registry. The method is retained to match the required public API.
