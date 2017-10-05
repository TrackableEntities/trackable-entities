import { TrackableEntity, TrackableSet } from 'trackable-entities';
import { CustomerSetting } from './customersetting';
import { Order } from './order';

export class Customer extends TrackableEntity {

  customerId: string;
  companyName: string;
  contactName: string;
  city: string;
  country: string;
  customerSetting: CustomerSetting;
  orders: TrackableSet<Order>;

  constructor() {
    super();
    return super.proxify(this);
  }
}
