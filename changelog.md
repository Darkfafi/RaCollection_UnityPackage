# Changelog RaCollection

## v1.3.0 - 31/03/2023
* Added Disposing functionality to RaCollection & RaElementCollection
* Added Sorting functionality to RaCollection & RaElementCollection
* Made it so the Added / Removed item callbacks no longer pass the mutable list
* Added non-index variants of the ForEach / ForEachReversed util methods
* Added RaPriorityCollection, which is a RaCollection which is sorted by priority

## v1.2.0 - 13/02/2023
* Added RaLocator, which is a RaElementCollection which can contain every type of entry

## v1.1.2 - 29/01/2023
* Reintroduced TryGetItem for safe explicit casting for RaElementCollections

## v1.1.1 - 29/01/2023
* Reintroduced TryGetItem and GetItems which were present in v1.0.0. So TryFindItem can be used for interfaces, and TryGetItem can be used for safe explicit casting

## v1.1.0 - 29/01/2023
* Moved util methods to the static class RaCollectionUtils so all IList & IReadOnlyList inheritors can benefit from them
* The casting TryGetItem & GetItems are renamed to TryFindItem & FindItems and made it so they don't expect T to derives from the list class (so interfaces can also be looked-up)

## v1.0.0 - 22/01/2023
* Initial Release
