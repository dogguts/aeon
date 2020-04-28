# Welcome to the Aeon repository!

Aeon Repository is a library implementing a **generic repository pattern** for **Entity Framework Core** specifically designed to be used with **dependency injection**.

Some specifics of this implementation:
* Designed for dependency injection
* UnitOfWork:  no save method on the repository 
	* Facilitates transactions over multiple repositories
	* Flexibility to perform additional actions on Commit 
* No IQueryable exposure: filtering is done through the specification pattern

## Quick links
- **[Examples](/examples/)**
- **[API Documentation](/api/)**
- **[GitHub repository](https://github.com/dogguts/aeon)**

## Getting started
For more information about installing and getting started with this library read the [Basics/Setup example](xref:example_basic_setup)

## Further reading
- [https://programmingwithmosh.com/net/common-mistakes-with-the-repository-pattern/](https://programmingwithmosh.com/net/common-mistakes-with-the-repository-pattern/)
- [https://deviq.com/repository-pattern/](https://deviq.com/repository-pattern/)
