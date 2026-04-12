namespace LDSTest.Shared
{
    public class DTO
    {
        public class Room
        {
            public int Id { get; set; }
            public int RoomNumber { get; set; }
            public int Price { get; set; }
        }
        public class Customer
        {
            public int Id { get; set; }
            public required string Name { get; set; }
        }
        public class Booking
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int RoomNumber { get; set; }
        }
    }
}