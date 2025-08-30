# شرح مفصل لمشروع LinkDev.Talabat

## صفحة الغلاف

**مشروع LinkDev.Talabat**  
*تحليل معماري شامل وتوثيق مفصل*  
*تاريخ التحليل: 22 أغسطس 2025*

---

## فهرس المحتويات

1. [نظرة معمارية عامة](#نظرة-معمارية-عامة)
2. [تحليل المشاريع](#تحليل-المشاريع)
   - [مشروع Domain](#مشروع-domain)
   - [مشروع Application](#مشروع-application)
   - [مشروع Infrastructure](#مشروع-infrastructure)
   - [مشروع APIs](#مشروع-apis)
   - [مشروع Dashboard](#مشروع-dashboard)
   - [مشروع Client](#مشروع-client)
3. [المفاهيم المستخدمة](#المفاهيم-المستخدمة)
4. [التدفق التشغيلي](#التدفق-التشغيلي)
5. [الأداء والأمان](#الأداء-والأمان)
6. [قاموس المصطلحات](#قاموس-المصطلحات)

---

## نظرة معمارية عامة

### هيكل الحل (Solution Structure)

المشروع يتبع **معمارية Clean Architecture** مع تقسيم واضح للمسؤوليات:

```
LinkDev.Talabat/
├── src/
│   ├── APIs/                    # طبقة العرض (Presentation Layer)
│   │   └── LinkDev.Talabat.APIs
│   ├── Core/                    # طبقة الأعمال (Business Layer)
│   │   ├── LinkDev.Talabat.Core.Domain
│   │   ├── LinkDev.Talabat.Core.Application
│   │   └── LinkDev.Talabat.Core.Application.Abstraction
│   ├── Infrastructure/          # طبقة البنية التحتية
│   │   ├── LinkDev.Talabat.Infrastructure.Presistence
│   │   └── LinkDev.Talabat.Infrastruture
│   └── UI/                      # واجهات المستخدم
│       ├── Client/              # Angular Frontend
│       └── Dashboard/           # ASP.NET Core MVC Dashboard
└── test/                        # اختبارات (غير موجودة حالياً)
```

### الرسم التخطيطي للتدفق

```
HTTP Request → APIs Controller → Application Services → Domain Services → 
Unit of Work → Repository → Entity Framework → SQL Server Database
```

### المبادئ المعمارية المستخدمة

1. **Dependency Inversion Principle**: الطبقات العليا لا تعتمد على الطبقات السفلية
2. **Repository Pattern**: فصل منطق الوصول للبيانات
3. **Unit of Work Pattern**: إدارة المعاملات والاتصال بقاعدة البيانات
4. **Generic Repository**: إعادة استخدام الكود للعمليات الأساسية
5. **Entity Framework Core**: ORM للتعامل مع قاعدة البيانات

---

## تحليل المشاريع

### مشروع Domain

**الموقع**: `LinkDev.Talabat.Core.Domain/`

**الهدف**: يحتوي على الكيانات الأساسية والواجهات الأساسية للنظام.

#### الملفات الرئيسية:

##### 1. BaseEntity.cs
```csharp
public abstract class BaseEntity<TKey> where TKey : IEquatable<TKey>
{
    public required TKey Id { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public required string LastModifiedBy { get; set; }
    public DateTime LastModifiedOn { get; set; }
}
```

**الشرح سطر بسطر:**
- `public abstract class`: كلاس مجرد لا يمكن إنشاء نسخة منه مباشرة
- `BaseEntity<TKey>`: كلاس عام مع معامل نوع `TKey` للـ ID
- `where TKey : IEquatable<TKey>`: قيد يضمن أن نوع الـ ID يدعم المقارنة
- `public required TKey Id`: معرف فريد للكيان (مطلوب)
- `public required string CreatedBy`: اسم منشئ الكيان (مطلوب)
- `public DateTime CreatedOn`: تاريخ إنشاء الكيان
- `public required string LastModifiedBy`: اسم آخر معدل للكيان (مطلوب)
- `public DateTime LastModifiedOn`: تاريخ آخر تعديل

**الدور**: يوفر الخصائص الأساسية لجميع الكيانات في النظام (Audit Trail).

##### 2. IGenericRepository.cs
```csharp
public interface IGenericRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    Task<TEntity?> GetAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}
```

**الشرح سطر بسطر:**
- `public interface`: واجهة تحدد العقد للعمليات الأساسية
- `IGenericRepository<TEntity, TKey>`: واجهة عامة للتعامل مع أي كيان
- `where TEntity : BaseEntity<TKey>`: قيد يضمن أن الكيان يرث من BaseEntity
- `Task<TEntity?> GetAsync(TKey id)`: جلب كيان واحد بواسطة الـ ID
- `Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false)`: جلب جميع الكيانات
- `Task AddAsync(TEntity entity)`: إضافة كيان جديد
- `Task UpdateAsync(TEntity entity)`: تحديث كيان موجود
- `Task DeleteAsync(TEntity entity)`: حذف كيان

**الدور**: يحدد العمليات الأساسية للتعامل مع قاعدة البيانات.

##### 3. IUnitOfWork.cs
```csharp
public interface IUnitOfWork : IAsyncDisposable
{
    public IGenericRepository<Product, int> ProductRepository { get; set; }
    public IGenericRepository<ProductBrand, int> BrandRepository { get; set; }
    public IGenericRepository<ProductCategory, int> CategoryRepository { get; set; }
    Task<int> CompleteAsync();
}
```

**الشرح سطر بسطر:**
- `public interface IUnitOfWork`: واجهة لإدارة المعاملات
- `IAsyncDisposable`: واجهة للتخلص من الموارد بشكل غير متزامن
- `ProductRepository`: مستودع للتعامل مع المنتجات
- `BrandRepository`: مستودع للتعامل مع العلامات التجارية
- `CategoryRepository`: مستودع للتعامل مع الفئات
- `Task<int> CompleteAsync()`: حفظ التغييرات وإرجاع عدد الصفوف المتأثرة

**الدور**: ينسق بين المستودعات المختلفة ويضمن اتساق البيانات.

##### 4. الكيانات (Entities)

###### Product.cs
```csharp
public class Product : BaseEntity<int>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? PictureUrL { get; set; }
    public decimal Price { get; set; }
    public int? BrandId { get; set; }
    public ProductBrand? Brand { get; set; }
    public int? CategoryId { get; set; }
    public virtual ProductCategory? Category { get; set; }
}
```

**الشرح سطر بسطر:**
- `public class Product : BaseEntity<int>`: كيان المنتج يرث من BaseEntity مع ID من نوع int
- `public required string Name`: اسم المنتج (مطلوب)
- `public required string Description`: وصف المنتج (مطلوب)
- `public string? PictureUrL`: رابط صورة المنتج (اختياري)
- `public decimal Price`: سعر المنتج
- `public int? BrandId`: معرف العلامة التجارية (مفتاح خارجي)
- `public ProductBrand? Brand`: علاقة مع العلامة التجارية (Navigation Property)
- `public int? CategoryId`: معرف الفئة (مفتاح خارجي)
- `public virtual ProductCategory? Category`: علاقة مع الفئة (Navigation Property)

**الدور**: يمثل المنتج في النظام مع علاقاته.

###### ProductBrand.cs
```csharp
public class ProductBrand : BaseEntity<int>
{
    public required string Name { get; set; }
    public ICollection<Product> products { get; set; } = new HashSet<Product>();
}
```

**الشرح سطر بسطر:**
- `public class ProductBrand : BaseEntity<int>`: كيان العلامة التجارية
- `public required string Name`: اسم العلامة التجارية (مطلوب)
- `public ICollection<Product> products`: مجموعة المنتجات التابعة لهذه العلامة
- `= new HashSet<Product>()`: تهيئة المجموعة كـ HashSet لتجنب التكرار

**الدور**: يمثل العلامة التجارية للمنتجات.

###### ProductCategory.cs
```csharp
public class ProductCategory : BaseEntity<int>
{
    public required string Name { get; set; }
}
```

**الشرح سطر بسطر:**
- `public class ProductCategory : BaseEntity<int>`: كيان فئة المنتج
- `public required string Name`: اسم الفئة (مطلوب)

**الدور**: يمثل فئة المنتج في النظام.

---

### مشروع Application

**الموقع**: `LinkDev.Talabat.Core.Application/` و `LinkDev.Talabat.Core.Application.Abstraction/`

**الهدف**: يحتوي على منطق الأعمال والخدمات.

**ملاحظة**: هذا المشروع فارغ حالياً، مما يشير إلى أن المشروع في مرحلة مبكرة من التطوير.

---

### مشروع Infrastructure

**الموقع**: `LinkDev.Talabat.Infrastructure.Presistence/`

**الهدف**: يحتوي على تنفيذ الوصول للبيانات وإدارة قاعدة البيانات.

#### الملفات الرئيسية:

##### 1. StoreContext.cs
```csharp
public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions<StoreContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyInformations).Assembly);
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductBrand> Brands { get; set; }
    public DbSet<ProductCategory> Categories { get; set; }
}
```

**الشرح سطر بسطر:**
- `public class StoreContext : DbContext`: كلاس السياق الرئيسي لقاعدة البيانات
- `public StoreContext(DbContextOptions<StoreContext> options)`: constructor يقبل خيارات التكوين
- `: base(options)`: تمرير الخيارات للكلاس الأساسي
- `protected override void OnModelCreating(ModelBuilder modelBuilder)`: تجاوز طريقة إنشاء النموذج
- `modelBuilder.ApplyConfigurationsFromAssembly`: تطبيق جميع التكوينات من المجمع
- `public DbSet<Product> Products`: جدول المنتجات في قاعدة البيانات
- `public DbSet<ProductBrand> Brands`: جدول العلامات التجارية
- `public DbSet<ProductCategory> Categories`: جدول الفئات

**الدور**: يمثل قاعدة البيانات ويحدد الجداول والعلاقات.

##### 2. GenericRepository.cs
```csharp
internal class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly StoreContext _dbContext;

    public GenericRepository(StoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity);
    }

    public Task DeleteAsync(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false)
    {
        if (withTracking)
            return await _dbContext.Set<TEntity>().ToListAsync();

        return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
    }

    public async Task<TEntity?> GetAsync(TKey id)
    {
        return await _dbContext.Set<TEntity>().FindAsync(id);
    }

    public Task UpdateAsync(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }
}
```

**الشرح سطر بسطر:**
- `internal class`: كلاس داخلي (غير مرئي خارج المجمع)
- `GenericRepository<TEntity, TKey>`: تنفيذ الواجهة العامة
- `private readonly StoreContext _dbContext`: سياق قاعدة البيانات (للقراءة فقط)
- `public GenericRepository(StoreContext dbContext)`: constructor يحقن السياق
- `public async Task AddAsync(TEntity entity)`: إضافة كيان جديد بشكل غير متزامن
- `public Task DeleteAsync(TEntity entity)`: حذف كيان (متزامن)
- `public async Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false)`: جلب جميع الكيانات
- `withTracking`: إذا كان true، Entity Framework يتتبع التغييرات
- `AsNoTracking()`: عدم تتبع التغييرات لتحسين الأداء
- `public async Task<TEntity?> GetAsync(TKey id)`: جلب كيان واحد بواسطة الـ ID
- `public Task UpdateAsync(TEntity entity)`: تحديث كيان موجود

**الدور**: تنفيذ العمليات الأساسية على قاعدة البيانات.

##### 3. UnitOfWork.cs
```csharp
internal class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly StoreContext _dbContext;

    private readonly Lazy<IGenericRepository<Product, int>> _productRepository;
    private readonly Lazy<IGenericRepository<ProductBrand, int>> _brandRepository;
    private readonly Lazy<IGenericRepository<ProductCategory, int>> _categoryRepository;

    public UnitOfWork(StoreContext dbContext)
    {
        _dbContext = dbContext;
        _productRepository = new Lazy<IGenericRepository<Product, int>>(() => new GenericRepository<Product, int>(_dbContext));
        _brandRepository = new Lazy<IGenericRepository<ProductBrand, int>>(() => new GenericRepository<ProductBrand, int>(_dbContext));
        _categoryRepository = new Lazy<IGenericRepository<ProductCategory, int>>(() => new GenericRepository<ProductCategory, int>(_dbContext));
    }

    public IGenericRepository<Product, int> ProductRepository => _productRepository.Value;
    public IGenericRepository<ProductBrand, int> BrandRepository => _brandRepository.Value;
    public IGenericRepository<ProductCategory, int> CategoryRepository => _categoryRepository.Value;

    public async Task<int> CompleteAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}
```

**الشرح سطر بسطر:**
- `internal class UnitOfWork`: تنفيذ نمط Unit of Work
- `IUnitOfWork, IAsyncDisposable`: تنفيذ الواجهات المطلوبة
- `private readonly StoreContext _dbContext`: سياق قاعدة البيانات
- `Lazy<IGenericRepository<Product, int>> _productRepository`: مستودع كسول للمنتجات
- `Lazy`: نمط التحميل الكسول (لا يتم إنشاء الكائن إلا عند الحاجة)
- `public UnitOfWork(StoreContext dbContext)`: constructor يحقن السياق
- `new Lazy<...>(() => new GenericRepository<...>(_dbContext))`: إنشاء مستودع كسول
- `public IGenericRepository<Product, int> ProductRepository => _productRepository.Value`: خاصية تعيد المستودع
- `public async Task<int> CompleteAsync()`: حفظ جميع التغييرات
- `public async ValueTask DisposeAsync()`: التخلص من الموارد

**الدور**: ينسق بين المستودعات المختلفة ويضمن اتساق البيانات.

##### 4. StoreContextIntializer.cs
```csharp
internal class StoreContextIntializer : IStoreContextIntializer
{
    private readonly StoreContext _dbContext;

    public StoreContextIntializer(StoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task IntializeAsync()
    {
        var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
            await _dbContext.Database.MigrateAsync();
    }

    public async Task SeedAsync()
    {
        #region Brands
        if (!_dbContext.Brands.Any())
        {
            var brandsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../LinkDev.Talabat.Infrastructure/Data/Seeds/brands.json"
            );

            var brandsData = await File.ReadAllTextAsync(brandsPath);
            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

            if (brands != null && brands.Any())
            {
                await _dbContext.Set<ProductBrand>().AddRangeAsync(brands);
                await _dbContext.SaveChangesAsync();
            }
        }
        #endregion

        // ... Categories and Products seeding
    }
}
```

**الشرح سطر بسطر:**
- `internal class StoreContextIntializer`: كلاس تهيئة قاعدة البيانات
- `IStoreContextIntializer`: تنفيذ واجهة التهيئة
- `private readonly StoreContext _dbContext`: سياق قاعدة البيانات
- `public async Task IntializeAsync()`: تهيئة قاعدة البيانات
- `GetPendingMigrationsAsync()`: الحصول على الهجرات المعلقة
- `MigrateAsync()`: تطبيق الهجرات المعلقة
- `public async Task SeedAsync()`: ملء قاعدة البيانات بالبيانات الأولية
- `if (!_dbContext.Brands.Any())`: التحقق من وجود بيانات
- `Path.Combine()`: دمج مسارات الملفات
- `File.ReadAllTextAsync()`: قراءة محتوى الملف
- `JsonSerializer.Deserialize()`: تحويل JSON إلى كائنات
- `AddRangeAsync()`: إضافة مجموعة من الكيانات
- `SaveChangesAsync()`: حفظ التغييرات

**الدور**: تهيئة قاعدة البيانات وملؤها بالبيانات الأولية.

##### 5. DependencyInjection.cs
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StoreContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("StoreContext"));
        });

        services.AddScoped<IStoreContextIntializer, StoreContextIntializer>();
        services.AddScoped(typeof(IStoreContextIntializer), typeof(StoreContextIntializer));
        return services;
    }
}
```

**الشرح سطر بسطر:**
- `public static class DependencyInjection`: كلاس ثابت لإعداد Dependency Injection
- `public static IServiceCollection AddPersistenceServices`: طريقة إضافية لإعداد الخدمات
- `this IServiceCollection services`: طريقة إضافية (Extension Method)
- `services.AddDbContext<StoreContext>`: تسجيل سياق قاعدة البيانات
- `options.UseSqlServer()`: استخدام SQL Server كقاعدة بيانات
- `configuration.GetConnectionString("StoreContext")`: الحصول على سلسلة الاتصال
- `services.AddScoped<IStoreContextIntializer, StoreContextIntializer>()`: تسجيل خدمة التهيئة
- `AddScoped`: نطاق الخدمة (مثيل واحد لكل طلب HTTP)

**الدور**: إعداد Dependency Injection للخدمات المتعلقة بقاعدة البيانات.

---

### مشروع APIs

**الموقع**: `LinkDev.Talabat.APIs/`

**الهدف**: واجهة برمجة التطبيقات REST API.

#### الملفات الرئيسية:

##### 1. Program.cs
```csharp
public static async Task Main(string[] args)
{
    var webApplicationBuilder = WebApplication.CreateBuilder(args);

    #region Configure Services
    webApplicationBuilder.Services.AddControllers();
    webApplicationBuilder.Services.AddEndpointsApiExplorer();
    webApplicationBuilder.Services.AddSwaggerGen();
    webApplicationBuilder.Services.AddPersistenceServices(webApplicationBuilder.Configuration);
    #endregion

    var app = webApplicationBuilder.Build();

    #region Database Initializer
    await app.IntializeStoreContextAsync();
    #endregion

    #region Configure Kestrel Midllwares
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    #endregion

    app.Run();
}
```

**الشرح سطر بسطر:**
- `public static async Task Main(string[] args)`: نقطة الدخول الرئيسية للتطبيق
- `var webApplicationBuilder = WebApplication.CreateBuilder(args)`: إنشاء منشئ التطبيق
- `webApplicationBuilder.Services.AddControllers()`: إضافة خدمات Controllers
- `AddEndpointsApiExplorer()`: إضافة استكشاف نقاط النهاية
- `AddSwaggerGen()`: إضافة Swagger لتوثيق API
- `AddPersistenceServices()`: إضافة خدمات قاعدة البيانات
- `var app = webApplicationBuilder.Build()`: بناء التطبيق
- `await app.IntializeStoreContextAsync()`: تهيئة قاعدة البيانات
- `app.UseSwagger()`: استخدام Swagger في بيئة التطوير
- `app.UseHttpsRedirection()`: إعادة توجيه HTTP إلى HTTPS
- `app.UseAuthorization()`: إضافة التفويض
- `app.MapControllers()`: تعيين Controllers
- `app.Run()`: تشغيل التطبيق

**الدور**: نقطة الدخول الرئيسية وإعداد التطبيق.

##### 2. IntializerExtensions.cs
```csharp
public static class IntializerExtensions
{
    public static async Task<WebApplication> IntializeStoreContextAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var storeContextIntializer = services.GetRequiredService<IStoreContextIntializer>();
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();

        try
        {
            await storeContextIntializer.IntializeAsync();
            await storeContextIntializer.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogError(ex, "An error occurred during applying migrations or the Seeding of the Data");
        }

        return app;
    }
}
```

**الشرح سطر بسطر:**
- `public static class IntializerExtensions`: كلاس ثابت للطرق الإضافية
- `public static async Task<WebApplication> IntializeStoreContextAsync`: طريقة إضافية لتهيئة قاعدة البيانات
- `this WebApplication app`: طريقة إضافية على WebApplication
- `using var scope = app.Services.CreateScope()`: إنشاء نطاق للخدمات
- `var services = scope.ServiceProvider`: مزود الخدمات
- `GetRequiredService<IStoreContextIntializer>()`: الحصول على خدمة التهيئة
- `GetRequiredService<ILoggerFactory>()`: الحصول على مصنع التسجيل
- `await storeContextIntializer.IntializeAsync()`: تهيئة قاعدة البيانات
- `await storeContextIntializer.SeedAsync()`: ملء البيانات الأولية
- `catch (Exception ex)`: معالجة الأخطاء
- `logger.LogError()`: تسجيل الخطأ
- `return app`: إرجاع التطبيق للاستمرار في التكوين

**الدور**: تهيئة قاعدة البيانات عند بدء التطبيق.

##### 3. WeatherForecastController.cs
```csharp
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
```

**الشرح سطر بسطر:**
- `[ApiController]`: سمة تحدد أن هذا Controller للـ API
- `[Route("[controller]")]`: مسار الـ Controller (WeatherForecast)
- `public class WeatherForecastController : ControllerBase`: Controller للتنبؤ بالطقس
- `private static readonly string[] Summaries`: مصفوفة ثابتة للقراءة فقط من الأوصاف
- `private readonly ILogger<WeatherForecastController> _logger`: مسجل الأحداث
- `public WeatherForecastController(ILogger<WeatherForecastController> logger)`: constructor يحقن المسجل
- `[HttpGet(Name = "GetWeatherForecast")]`: نقطة نهاية GET
- `public IEnumerable<WeatherForecast> Get()`: طريقة جلب التنبؤات
- `Enumerable.Range(1, 5)`: إنشاء نطاق من 1 إلى 5
- `Select(index => new WeatherForecast {...})`: تحويل كل رقم إلى تنبؤ
- `DateOnly.FromDateTime(DateTime.Now.AddDays(index))`: تاريخ اليوم + عدد الأيام
- `Random.Shared.Next(-20, 55)`: درجة حرارة عشوائية
- `Summaries[Random.Shared.Next(Summaries.Length)]`: وصف عشوائي
- `.ToArray()`: تحويل النتيجة إلى مصفوفة

**الدور**: Controller تجريبي للتنبؤ بالطقس (مثال من ASP.NET Core).

##### 4. appsettings.json
```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "StoreContext": "Server=.;Database=Talabat.APIs;Trusted_Connection=True;TrustServerCertificate=True;"
    }
}
```

**الشرح سطر بسطر:**
- `"Logging"`: إعدادات التسجيل
- `"Default": "Information"`: مستوى التسجيل الافتراضي
- `"Microsoft.AspNetCore": "Warning"`: مستوى تسجيل ASP.NET Core
- `"AllowedHosts": "*"`: السماح لجميع المضيفين
- `"ConnectionStrings"`: سلاسل الاتصال بقواعد البيانات
- `"StoreContext"`: اسم سلسلة الاتصال
- `"Server=."`: الخادم المحلي
- `"Database=Talabat.APIs"`: اسم قاعدة البيانات
- `"Trusted_Connection=True"`: استخدام مصادقة Windows
- `"TrustServerCertificate=True"`: الثقة في شهادة الخادم

**الدور**: ملف إعدادات التطبيق.

---

### مشروع Dashboard

**الموقع**: `LinkDev.Talabat.Dashboard/`

**الهدف**: لوحة تحكم باستخدام ASP.NET Core MVC.

#### الملفات الرئيسية:

##### 1. Program.cs
```csharp
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
```

**الشرح سطر بسطر:**
- `public static void Main(string[] args)`: نقطة الدخول الرئيسية
- `var builder = WebApplication.CreateBuilder(args)`: إنشاء منشئ التطبيق
- `builder.Services.AddControllersWithViews()`: إضافة خدمات MVC
- `var app = builder.Build()`: بناء التطبيق
- `if (!app.Environment.IsDevelopment())`: التحقق من بيئة الإنتاج
- `app.UseExceptionHandler("/Home/Error")`: معالج الأخطاء
- `app.UseHsts()`: HTTP Strict Transport Security
- `app.UseHttpsRedirection()`: إعادة توجيه HTTP إلى HTTPS
- `app.UseStaticFiles()`: الملفات الثابتة (CSS, JS, Images)
- `app.UseRouting()`: التوجيه
- `app.UseAuthorization()`: التفويض
- `app.MapControllerRoute()`: تعيين مسار Controller الافتراضي
- `pattern: "{controller=Home}/{action=Index}/{id?}"`: نمط المسار الافتراضي

**الدور**: نقطة الدخول الرئيسية للوحة التحكم.

---

### مشروع Client

**الموقع**: `Talabat.Client/`

**الهدف**: واجهة مستخدم باستخدام Angular.

#### الملفات الرئيسية:

##### 1. package.json
```json
{
  "name": "talabat.client",
  "version": "0.0.0",
  "scripts": {
    "ng": "ng",
    "start": "ng serve --host=127.0.0.1",
    "build": "ng build",
    "watch": "ng build --watch --configuration development",
    "test": "ng test"
  },
  "dependencies": {
    "@angular/common": "^20.1.0",
    "@angular/compiler": "^20.1.0",
    "@angular/core": "^20.1.0",
    "@angular/forms": "^20.1.0",
    "@angular/platform-browser": "^20.1.0",
    "@angular/router": "^20.1.0",
    "rxjs": "~7.8.0",
    "tslib": "^2.3.0",
    "zone.js": "~0.15.0"
  }
}
```

**الشرح سطر بسطر:**
- `"name": "talabat.client"`: اسم المشروع
- `"version": "0.0.0"`: إصدار المشروع
- `"scripts"`: أوامر التشغيل
- `"ng": "ng"`: أمر Angular CLI
- `"start": "ng serve --host=127.0.0.1"`: تشغيل الخادم المحلي
- `"build": "ng build"`: بناء المشروع
- `"watch": "ng build --watch"`: البناء مع المراقبة
- `"test": "ng test"`: تشغيل الاختبارات
- `"dependencies"`: المكتبات المطلوبة
- `"@angular/common"`: المكتبات المشتركة لـ Angular
- `"@angular/core"`: النواة الأساسية لـ Angular
- `"@angular/forms"`: نماذج Angular
- `"@angular/platform-browser"`: منصة المتصفح
- `"@angular/router"`: التوجيه في Angular
- `"rxjs"`: مكتبة البرمجة التفاعلية
- `"zone.js"`: إدارة المناطق في Angular

**الدور**: ملف تكوين مشروع Angular.

---

## المفاهيم المستخدمة

### 1. Clean Architecture

**التعريف**: نمط معماري يفصل طبقات التطبيق ويقلل الاعتماديات.

**المكونات في هذا المشروع:**
- **Domain Layer**: الكيانات والواجهات الأساسية
- **Application Layer**: منطق الأعمال (فارغ حالياً)
- **Infrastructure Layer**: الوصول للبيانات والخدمات الخارجية
- **Presentation Layer**: واجهات المستخدم والـ APIs

### 2. Repository Pattern

**التعريف**: نمط يفصل منطق الوصول للبيانات عن منطق الأعمال.

**التطبيق في المشروع:**
```csharp
public interface IGenericRepository<TEntity, TKey>
{
    Task<TEntity?> GetAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}
```

**المزايا:**
- فصل منطق الوصول للبيانات
- سهولة الاختبار
- إعادة استخدام الكود
- تغيير مصدر البيانات بسهولة

### 3. Unit of Work Pattern

**التعريف**: نمط ينسق بين المستودعات المختلفة ويضمن اتساق البيانات.

**التطبيق في المشروع:**
```csharp
public interface IUnitOfWork : IAsyncDisposable
{
    public IGenericRepository<Product, int> ProductRepository { get; set; }
    public IGenericRepository<ProductBrand, int> BrandRepository { get; set; }
    public IGenericRepository<ProductCategory, int> CategoryRepository { get; set; }
    Task<int> CompleteAsync();
}
```

**المزايا:**
- إدارة المعاملات
- ضمان اتساق البيانات
- تنسيق العمليات المتعددة

### 4. Entity Framework Core

**التعريف**: ORM (Object-Relational Mapping) للتعامل مع قواعد البيانات.

**التطبيق في المشروع:**
```csharp
public class StoreContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductBrand> Brands { get; set; }
    public DbSet<ProductCategory> Categories { get; set; }
}
```

**المزايا:**
- التعامل مع قاعدة البيانات ككائنات
- إدارة العلاقات تلقائياً
- دعم LINQ
- إدارة الهجرات

### 5. Dependency Injection

**التعريف**: نمط يحقن الاعتماديات بدلاً من إنشائها داخلياً.

**التطبيق في المشروع:**
```csharp
services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("StoreContext"));
});
services.AddScoped<IStoreContextIntializer, StoreContextIntializer>();
```

**المزايا:**
- فصل الاعتماديات
- سهولة الاختبار
- إدارة دورة حياة الكائنات
- تقليل الاقتران

### 6. Generic Programming

**التعريف**: برمجة عامة تسمح بإعادة استخدام الكود مع أنواع مختلفة.

**التطبيق في المشروع:**
```csharp
public abstract class BaseEntity<TKey> where TKey : IEquatable<TKey>
public interface IGenericRepository<TEntity, TKey>
```

**المزايا:**
- إعادة استخدام الكود
- نوع آمن
- مرونة في التعامل مع أنواع مختلفة

---

## التدفق التشغيلي

### مثال: رحلة طلب جلب المنتجات

```
1. HTTP Request → GET /api/products
2. Controller → ProductsController.Get()
3. Service → ProductService.GetAllProducts()
4. Unit of Work → UnitOfWork.ProductRepository.GetAllAsync()
5. Repository → GenericRepository<Product, int>.GetAllAsync()
6. Entity Framework → StoreContext.Products.ToListAsync()
7. SQL Server → SELECT * FROM Products
8. Response → JSON Array of Products
```

### نقاط التحقق من الصحة

**حالياً**: لا توجد تحققات من الصحة محددة في المشروع.

**التحسينات المقترحة:**
- إضافة FluentValidation
- إضافة Data Annotations
- إضافة Custom Validation Attributes

---

## الأداء والأمان

### نقاط الأداء

**1. Entity Framework Tracking:**
```csharp
public async Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false)
{
    if (withTracking)
        return await _dbContext.Set<TEntity>().ToListAsync();

    return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
}
```

**التحسين**: استخدام `AsNoTracking()` لتحسين الأداء عند القراءة فقط.

**2. Lazy Loading:**
```csharp
private readonly Lazy<IGenericRepository<Product, int>> _productRepository;
```

**التحسين**: استخدام Lazy Loading لتأجيل إنشاء الكائنات.

**3. Connection String:**
```json
"ConnectionStrings": {
    "StoreContext": "Server=.;Database=Talabat.APIs;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**التحسين**: استخدام Connection Pooling وتحسين إعدادات SQL Server.

### الاعتبارات الأمنية

**حالياً**: لا توجد إعدادات أمان محددة.

**التحسينات المقترحة:**
- إضافة Authentication/Authorization
- تشفير البيانات الحساسة
- استخدام HTTPS
- إضافة Rate Limiting
- معالجة الأخطاء بشكل آمن

---

## قاموس المصطلحات

### مصطلحات عامة

1. **Clean Architecture**: معمارية نظيفة تفصل طبقات التطبيق
2. **Repository Pattern**: نمط المستودع لفصل منطق الوصول للبيانات
3. **Unit of Work**: نمط وحدة العمل لتنسيق العمليات
4. **Dependency Injection**: حقن الاعتماديات
5. **Entity Framework Core**: ORM للتعامل مع قواعد البيانات
6. **Generic Programming**: البرمجة العامة
7. **Lazy Loading**: التحميل الكسول
8. **Async/Await**: البرمجة غير المتزامنة

### مصطلحات تقنية

1. **DbContext**: سياق قاعدة البيانات في Entity Framework
2. **DbSet**: مجموعة الكيانات في قاعدة البيانات
3. **Migration**: هجرة قاعدة البيانات
4. **Scoped**: نطاق الخدمة في Dependency Injection
5. **Singleton**: نمط التصميم الوحيد
6. **Transient**: عابر (مثيل جديد في كل مرة)
7. **LINQ**: Language Integrated Query
8. **ORM**: Object-Relational Mapping

### روابط الملفات

- **BaseEntity**: `LinkDev.Talabat.Core.Domain/Common/BaseEntity.cs`
- **IGenericRepository**: `LinkDev.Talabat.Core.Domain/Contracts/IGenericRepository.cs`
- **IUnitOfWork**: `LinkDev.Talabat.Core.Domain/Contracts/IUnitOfWork.cs`
- **StoreContext**: `LinkDev.Talabat.Infrastructure.Presistence/Data/StoreContext.cs`
- **GenericRepository**: `LinkDev.Talabat.Infrastructure.Presistence/Repositories/GenericRepository.cs`
- **UnitOfWork**: `LinkDev.Talabat.Infrastructure.Presistence/UnitOfWork/UnitOfWork.cs`
- **Program.cs**: `LinkDev.Talabat.APIs/Program.cs`

---

## أوامر التشغيل

### أوامر .NET

```bash
# تشغيل مشروع APIs
cd LinkDev.Talabat.APIs
dotnet run

# تشغيل مشروع Dashboard
cd LinkDev.Talabat.Dashboard
dotnet run

# إنشاء هجرة جديدة
dotnet ef migrations add InitialCreate

# تطبيق الهجرات
dotnet ef database update

# إزالة آخر هجرة
dotnet ef migrations remove
```

### أوامر Angular

```bash
# تشغيل مشروع Client
cd Talabat.Client
npm install
npm start

# بناء المشروع
npm run build

# تشغيل الاختبارات
npm test
```

---

## متغيرات البيئة

### appsettings.Development.json
```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    }
}
```

### appsettings.json
```json
{
    "ConnectionStrings": {
        "StoreContext": "Server=.;Database=Talabat.APIs;Trusted_Connection=True;TrustServerCertificate=True;"
    }
}
```

---

## خريطة التنقل

| الملف | الصفحة | الوصف |
|-------|--------|-------|
| BaseEntity.cs | 3 | الكلاس الأساسي للكيانات |
| IGenericRepository.cs | 4 | واجهة المستودع العام |
| IUnitOfWork.cs | 5 | واجهة وحدة العمل |
| Product.cs | 6 | كيان المنتج |
| StoreContext.cs | 8 | سياق قاعدة البيانات |
| GenericRepository.cs | 9 | تنفيذ المستودع العام |
| UnitOfWork.cs | 10 | تنفيذ وحدة العمل |
| Program.cs (APIs) | 12 | نقطة دخول APIs |
| Program.cs (Dashboard) | 15 | نقطة دخول Dashboard |
| package.json | 17 | تكوين مشروع Angular |

---

## الخلاصة

مشروع LinkDev.Talabat هو تطبيق متعدد الطبقات يتبع معمارية Clean Architecture. المشروع منظم بشكل جيد ويستخدم أنماط تصميم قوية مثل Repository Pattern و Unit of Work Pattern. 

**النقاط القوية:**
- معمارية واضحة ومنظمة
- فصل المسؤوليات
- استخدام أنماط تصميم قوية
- دعم Entity Framework Core
- إعداد Dependency Injection

**التحسينات المقترحة:**
- إضافة منطق الأعمال في طبقة Application
- إضافة التحقق من الصحة
- إضافة Authentication/Authorization
- إضافة الاختبارات
- تحسين الأداء والأمان

المشروع في مرحلة مبكرة من التطوير ويحتاج إلى المزيد من التطوير لإكمال الوظائف المطلوبة. 