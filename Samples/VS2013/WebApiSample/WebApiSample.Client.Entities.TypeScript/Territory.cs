import { TrackableEntity, TrackableSet } from 'trackable-entities';
import { Employee } from './employee';

export class Territory extends TrackableEntity {

  territoryId: string;
  territoryDescription: string;
  employees: TrackableSet<Employee>;

  constructor() {
    super();
    return super.proxify(this);
  }
}
