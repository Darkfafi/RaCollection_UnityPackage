# Changelog RaCollection

## v1.1.1 - 29/01/2023
* Reintroduced TryGetItem and GetItems which were present in v1.0.0. So TryFindItem can be used for interfaces, and TryGetItem can be used for safe explicit casting

## v1.1.0 - 29/01/2023
* Moved util methods to the static class RaCollectionUtils so all IList & IReadOnlyList inheritors can benefit from them
* The casting TryGetItem & GetItems are renamed to TryFindItem & FindItems and made it so they don't expect T to derives from the list class (so interfaces can also be looked-up)

## v1.0.0 - 22/01/2023
* Initial Release
