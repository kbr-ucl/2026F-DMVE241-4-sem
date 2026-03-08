using EventCatalog.Facade.DTOs;
namespace EventCatalog.Facade.Interfaces;

/// <summary>Command: Opretter event. Returnerer Task (void) jf. CQS.</summary>
public interface ICreateEventUseCase { Task Execute(CreateEventRequest request); }
