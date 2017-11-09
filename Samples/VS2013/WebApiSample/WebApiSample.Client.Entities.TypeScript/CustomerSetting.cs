import { TrackableEntity, TrackableSet } from 'trackable-entities';
import { Customer } from './customer';

export class CustomerSetting extends TrackableEntity {

  customerId: string;
  setting: string;
  customer: Customer;

  constructor() {
    super();
    return super.proxify(this);
  }
}
