import { TrackableEntity, TrackableSet } from 'trackable-entities';
import { Customer } from './customer';
import { OrderDetail } from './orderdetail';

export class Order extends TrackableEntity {

  orderId: number;
  customerId: string;
  orderDate: Date | null;
  shippedDate: Date | null;
  shipVia: number | null;
  freight: number | null;
  customer: Customer;
  orderDetails: TrackableSet<OrderDetail>;

  constructor() {
    super();
    return super.proxify(this);
  }
}
