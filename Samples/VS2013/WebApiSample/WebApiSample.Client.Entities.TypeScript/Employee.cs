import { TrackableEntity, TrackableSet } from 'trackable-entities';
import { Territory } from './territory';

export class Employee extends TrackableEntity {

  employeeId: number;
  lastName: string;
  firstName: string;
  birthDate: Date | null;
  hireDate: Date | null;
  city: string;
  country: string;
  territories: TrackableSet<Territory>;

  constructor() {
    super();
    return super.proxify(this);
  }
}
