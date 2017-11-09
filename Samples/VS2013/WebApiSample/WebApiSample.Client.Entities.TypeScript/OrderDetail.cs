import { TrackableEntity, TrackableSet } from 'trackable-entities';
import { Order } from './order';
import { Product } from './product';

export class OrderDetail extends TrackableEntity {

  orderDetailId: number;
  orderId: number;
  productId: number;
  unitPrice: number;
  quantity: number;
  discount: float;
  order: Order;
  product: Product;

  constructor() {
    super();
    return super.proxify(this);
  }
}
